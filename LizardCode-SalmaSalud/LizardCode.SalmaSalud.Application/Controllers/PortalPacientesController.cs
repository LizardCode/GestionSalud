using LizardCode.Framework.Application.Common.Enums;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Application.Controllers.Base;
using LizardCode.Framework.Application.Helpers;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Login;
using LizardCode.SalmaSalud.Application.Models.Pacientes;
using LizardCode.SalmaSalud.Application.Models.PortalPacientes;
using LizardCode.SalmaSalud.Application.Models.TurnosSolicitud;
using LizardCode.SalmaSalud.Application.Startup;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Domain.Enums;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class PortalPacientesController : BaseController
    {
        private readonly IUsuariosBusiness _usuarios;
        private readonly IEmpresasBusiness _empresasBusiness;
        private readonly IPermisosBusiness _permisosBusiness;
        private readonly IEvolucionesBusiness _evolucionesBusiness;
        private readonly ITurnosBusiness _turnosBusiness;
        private readonly IPacientesBusiness _pacientesBusiness;
        private readonly IMemoryCache _memoryCache;
        private readonly ILookupsBusiness _lookupsBusiness;
        private readonly ITurnosSolicitudBusiness _turnosSolicitudBusiness;

        private string _captchaSiteKey = string.Empty;
        private string _captchaPrivateKey = string.Empty;
        private bool _captchaEnabled = false;

        private readonly string _cacheKey_TURNOS = "Paciente_Turnos_";
        private readonly string _cacheKey_EVOLUCIONES = "Paciente_Evoluciones_";

        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public PortalPacientesController(IUsuariosBusiness usuarios, IEmpresasBusiness empresasBusiness, 
                                            IPermisosBusiness permisosBusiness, IEvolucionesBusiness evolucionesBusiness, 
                                            ITurnosBusiness turnosBusiness, IPacientesBusiness pacientesBusiness, 
                                            IMemoryCache memoryCache, ILookupsBusiness lookupsBusiness,
                                            ITurnosSolicitudBusiness turnosSolicitudBusiness)
        {
            _usuarios = usuarios;
            _empresasBusiness = empresasBusiness;
            _permisosBusiness = permisosBusiness;
            _evolucionesBusiness = evolucionesBusiness;
            _turnosBusiness = turnosBusiness;
            _pacientesBusiness = pacientesBusiness;
            _memoryCache = memoryCache;
            _lookupsBusiness = lookupsBusiness;
            _turnosSolicitudBusiness = turnosSolicitudBusiness;

            _captchaSiteKey = "Captcha:SiteKey".FromAppSettings<string>(notFoundException: true);
            _captchaPrivateKey = "Captcha:PrivateKey".FromAppSettings<string>(notFoundException: true);
            _captchaEnabled = "Captcha:Enabled".FromAppSettings<bool>(notFoundException: true);
        }

        [Route("portal-pacientes")]
        [Authorize(Roles = "PACIENTE")]
        public async Task<ActionResult> Home()
        {
            TempData.Remove("MenuItem");
            TempData.Add("MenuItem", Url.Action("Index", "Home"));

            ViewBag.UsuarioSesion = _permisosBusiness.User ?? throw new Exception();
            ViewBag.Empresa = _permisosBusiness.User.Empresa ?? throw new Exception("Sin Empresa");
            ViewBag.CUIT = _permisosBusiness.User.CUIT ?? throw new Exception("Sin Empresa");

            if (_permisosBusiness.User.IdTipoUsuario == (int)TipoUsuario.Paciente)
            {
                //ViewBag.HomeTemplate = GetHomeTemplate(GetSubDomain());
                return View();
            }
            else
            {
                return View(GlobalSettings.PaginaInicial);
            }
        }

        #region Login Methods

        [Route("portal-pacientes/login")]
        public async Task<ActionResult> Login()
        {
            var model = new LoginViewModel { SiteKey = _captchaEnabled ? _captchaSiteKey : string.Empty };

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> SignIn(LoginViewModel model)
        {
            try
            {
                if (model.User.IsNull() || model.Pass.IsNull())
                    throw new Exception("Usuario y contraseña son obligatorios");

                if (await ExceedAttemptsByDocument(model.User))
                {
                    throw new Exception("Ha superado el número de intentos máximos. Espere 30 segundos para volver a intentarlo.");
                }

                if (_captchaEnabled)
                {
                    var validCaptcha = false;

                    if (Request.Form.ContainsKey("g-recaptcha-response"))
                    {
                        var captcha = Request.Form["g-recaptcha-response"].ToString();

                        validCaptcha = await ReCaptchaHelper.ValidarCaptchaV2(captcha);

                        if (!validCaptcha)
                        {
                            throw new RequiredCaptchaException();
                        }
                    }
                }

                await _permisosBusiness.SignInPacientes(HttpContext, model);

                return Json(() => Url.Action("Home", "PortalPacientes"));
            }
            catch (LoginFailedException ex)
            {
                return Json(ex);
            }
            catch (PasswordExpiredException ex)
            {
                return Json(ex.Code, Url.Action("ResetPassword", "Login"));
            }
            catch (UserNotFoundException ex)
            {
                return Json(ex);
            }
            catch (PermissionException ex)
            {
                return Json(ex);
            }
            catch (EmpresaNotDefinedException ex)
            {
                return Json(ex);
            }
            catch (RequiredCaptchaException ex)
            {
                return Json(ex);
            }
            catch (Exception ex)
            {
                return Json(0, ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> RequestAccessCode(LoginViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.User))
                    throw new Exception("Documento obligatorio");

                if (await ExceedAttemptsByDocument(model.User, "RequestAccessCode"))
                {
                    throw new Exception("Ha superado el número de intentos máximos. Espere 30 segundos para volver a intentarlo.");
                }

                if (_captchaEnabled)
                {
                    var validCaptcha = false;

                    if (Request.Form.ContainsKey("g-recaptcha-response"))
                    {
                        var captcha = Request.Form["g-recaptcha-response"].ToString();

                        validCaptcha = await ReCaptchaHelper.ValidarCaptchaV2(captcha);

                        if (!validCaptcha)
                        {
                            throw new RequiredCaptchaException();
                        }
                    }
                }

                var reqAccessCode = await RequestedAccessCode(model.User);
                if (reqAccessCode > 0)
                {
                    return Json(-2, $"Ya se ha enviado un código. Intente solicitar nuevamente en {reqAccessCode} minutos.");
                }

                //TODO GS: DESACTIVAR
                var accessCode = await _usuarios.RequestAccessCode(model.User);

                return Json(-1, accessCode);
            }
            catch (Exception ex)
            {
                await RemoveRequestedAccessCode(model.User);
                return Json(-2, ex.Message);
            }
        }

        private async Task<int> RequestedAccessCode(string documento)
        {
            int iReturn = 0;
            var cacheKey = "AccessCode_" + documento;

            DateTime? requestExpiration;
            _memoryCache.TryGetValue(cacheKey, out requestExpiration);
            if (requestExpiration != null)
            {
                if (requestExpiration > DateTime.Now)
                {
                    TimeSpan ts = requestExpiration.Value - DateTime.Now;
                    iReturn = (int)ts.TotalMinutes;
                }
            }
            else
            {
                requestExpiration = DateTime.Now.AddMinutes(30);
                _memoryCache.Set(cacheKey, requestExpiration, requestExpiration.Value);
            }

            return iReturn;
        }

        private async Task RemoveRequestedAccessCode(string documento)
        {
            var cacheKey = "AccessCode_" + documento;

            _memoryCache.Remove(cacheKey);
        }

        public new async Task<ActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/portal-pacientes/login");
        }

        #endregion

        [Route("portal-pacientes/turnos")]
        [Authorize(Roles = "PACIENTE")]
        public async Task<ActionResult> Turnos()
        {
            ViewBag.UsuarioSesion = _permisosBusiness.User ?? throw new Exception();
            ViewBag.Empresa = string.Empty;
            ViewBag.CUIT = string.Empty;

            var model = new TurnosSolicitudViewModel();

            #region caché validation 

            //var cacheKey = _cacheKey_TURNOS + _permisosBusiness.User.Login + "TODO:...";

            //_memoryCache.TryGetValue(cacheKey, out model);
            //if (model == null)
            //{
                var turnos = await _turnosSolicitudBusiness.GetTurnosPaciente();

                model = new TurnosSolicitudViewModel();
                model.Turnos = turnos?.ToList()?.OrderByDescending(o => o.IdTurnoSolicitud).ToList();

            //    _memoryCache.Set(cacheKey, model, DateTime.Now.AddMinutes(30));
            //}

            #endregion

            return View(model);
        }

        [Route("portal-pacientes/historia-clinica")]
        [Authorize(Roles = "PACIENTE")]
        public async Task<ActionResult> Evoluciones()
        {
            ViewBag.UsuarioSesion = _permisosBusiness.User ?? throw new Exception();
            ViewBag.Empresa = string.Empty;
            ViewBag.CUIT = string.Empty;

            var model = new List<EvolucionesViewModel>();
            var evoluciones = new List<Custom.Evolucion>();

            #region caché validation 

            var cacheKey = _cacheKey_EVOLUCIONES + _permisosBusiness.User.Login;

            _memoryCache.TryGetValue(cacheKey, out evoluciones);
            if (evoluciones == null)
            {
                evoluciones = await _evolucionesBusiness.GetEvolucionesPaciente();
                _memoryCache.Set(cacheKey, evoluciones, DateTime.Now.AddMinutes(30));
            }

            #endregion

            //TempData.Remove("MenuItem");
            //TempData.Add("MenuItem", Url.Action("Index", "Evoluciones"));
            if (evoluciones != null && evoluciones.Count > 0)
            {
                evoluciones = evoluciones.OrderByDescending(o => o.IdEvolucion).ToList();
                foreach (var evolucion in evoluciones)
                {
                    model.Add(new EvolucionesViewModel
                    {
                        IdEvolucion = evolucion.IdEvolucion,
                        Fecha = evolucion.Fecha,
                        Empresa = evolucion.Empresa,
                        Especialidad = evolucion.Especialidad
                    });
                }
            }


            return View(model);
        }

        [Route("portal-pacientes/mis-datos")]
        [Authorize(Roles = "PACIENTE")]
        public async Task<ActionResult> MisDatos()
        {
            ViewBag.UsuarioSesion = _permisosBusiness.User ?? throw new Exception();
            ViewBag.Empresa = string.Empty;
            ViewBag.CUIT = string.Empty;

            var paciente = await _pacientesBusiness.Get(_permisosBusiness.User.IdPaciente);

            var model = new MisDatosViewModel { FechaNacimiento = paciente.FechaNacimiento, Email = paciente.Email, Telefono = paciente.Telefono };

            //TempData.Remove("MenuItem");
            //TempData.Add("MenuItem", Url.Action("Index", "Evoluciones"));

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "PACIENTE")]
        public async Task<ActionResult> ActualizarDatos(PacienteViewModel model)
        {
            await _pacientesBusiness.UpdateFromPortal(model);
            return Json(true);
        }

        [Route("portal-pacientes/nueva-solicitud")]
        [Authorize(Roles = "PACIENTE")]
        public async Task<ActionResult> NuevaSolicitud()
        {
            ViewBag.UsuarioSesion = _permisosBusiness.User ?? throw new Exception();
            ViewBag.Empresa = string.Empty;
            ViewBag.CUIT = string.Empty;

            var dias = Utilities.EnumToDictionary<Dias>();
            var rangos = Utilities.EnumToDictionary<RangoHorario>();
            var especialidades = (await _lookupsBusiness.GetAllEspecialidades()).ToList();

            var model = new NuevaSolicitudViewModel
            {
                MaestroDias = dias
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroRangosHorarios = rangos
                    .ToDropDownList(descriptionIncludesKey: false),

                MaestroEspecialidades = especialidades
                    .ToDropDownList(k => k.IdEspecialidad, t => t.Descripcion, descriptionIncludesKey: false)
            };

            return View("NuevaSolicitud", model);
        }

        [HttpPost]
        [Authorize(Roles = "PACIENTE")]
        public async Task<ActionResult> GuardarSolicitud(NuevaSolicitudViewModel model)
        {
            model.IdPaciente = _permisosBusiness.User.IdPaciente;

            await _turnosSolicitudBusiness.Solicitar(model);
            return Json(true);
        }

        #region Private Methods

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            DetectLoggedUser(context);

            ViewBag.SubDomain = GetSubDomain();
        }

        private ActionResult ActivateMenuItem(string view = null, object model = null)
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            TempData.Remove("MenuItem");
            TempData.Add("MenuItem", Url.Action(action, controller).TrimStart('/'));

            return View(viewName: view, model: model);
        }

        private void DetectLoggedUser(ActionExecutingContext context)
        {
            var controllerName = ControllerContext.ActionDescriptor.ControllerName;
            var actionName = ControllerContext.ActionDescriptor.ActionName;

            var controller = Assembly
                .GetExecutingAssembly()
                .DefinedTypes
                .FirstOrDefault(f => f.Name.Equals($"{controllerName}Controller", StringComparison.InvariantCultureIgnoreCase));

            if (controller != null)
            {
                var controllerAuthorize = controller.GetCustomAttribute(typeof(AuthorizeAttribute));

                if (controllerAuthorize == null)
                {
                    var action = controller
                        .GetMethods()
                        .FirstOrDefault(f => f.Name.Equals($"{actionName}", StringComparison.InvariantCultureIgnoreCase));

                    if (action != null)
                    {
                        var actionAuthorize = action.GetCustomAttribute(typeof(AuthorizeAttribute), false);

                        if (actionAuthorize == null)
                        {
                            return;
                        }
                    }
                }
            }

            try
            {
                ViewBag.UsuarioSesion = _permisosBusiness.User ?? throw new Exception();
                ViewBag.Empresa = string.Empty;
                ViewBag.CUIT = string.Empty;
            }
            catch
            {
                context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).GetAwaiter().GetResult();
                context.Result = Redirect("/portal-pacientes/login"); // RedirectToAction("Login", "PortalPacientes");
            }
        }

        private string GetSubDomain()
        {
            var subDomain = string.Empty;

            var host = HttpContextHelper.Current.Request.Host.Host;

            if (!string.IsNullOrWhiteSpace(host))
            {
                subDomain = host.Split('.')[0];
            }

            return subDomain.Trim().ToLower();
        }


        private async Task<bool> ExceedAttemptsByDocument(string documento, string method = "")
        {
            bool bReturn = false;

            if (!_captchaEnabled)
                return false;

            var ip = HttpContext.Request.GetIP();
            var cacheKey = documento + "_" + ip + "_" + method;

            LoginAttempt loginAttemp;
            _memoryCache.TryGetValue(cacheKey, out loginAttemp);
            if (loginAttemp != null)
            {
                loginAttemp.Intento++;

                if (loginAttemp.Intento > 10)
                {
                    bReturn = true;
                }

                _memoryCache.Set(cacheKey, loginAttemp, DateTime.Now.AddSeconds(30));
            }
            else
            {
                loginAttemp = new LoginAttempt { Usuario = documento, Ip = ip, Intento = 1 };
                _memoryCache.Set(cacheKey, loginAttemp, DateTime.Now.AddSeconds(30));
            }

            return bReturn;
        }

        private string GetHomeTemplate(string subdomain)
        {
            var crp = AppDomain.CurrentDomain.GetData("WebRootPath").ToString();

            var path = Path.Combine(crp, "template", "portal", $"{subdomain}_home.html");

            string template = string.Empty;

            if (!System.IO.File.Exists(path))
            {
                _logger.Warn($"No se encontró el template. template\\portal\\{subdomain}_home.html. Path: {path}. Crp: {crp}.");
            }

            using (StreamReader reader = new(path))
                template = reader.ReadToEnd();


            return template;
        }

        #endregion
    }
}
