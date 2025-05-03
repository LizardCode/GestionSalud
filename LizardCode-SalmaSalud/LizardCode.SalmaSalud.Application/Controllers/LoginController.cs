using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.Framework.Application.Helpers;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Login;
using System;
using System.Linq;
using System.Threading.Tasks;
using LizardCode.Framework.Application.Controllers.Base;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    internal class LoginAttempt
    {
        public string Usuario { get; set; }
        public string Ip { get; set; }
        public int Intento { get; set; }
    }

    public class LoginController : BaseController
    {
        private readonly IUsuariosBusiness _usuarios;
        private readonly IEmpresasBusiness _empresasBusiness;
        private readonly IPermisosBusiness _permisosBusiness;

        private IMemoryCache memoryCache;
        private string _captchaSiteKey = string.Empty;
        private string _captchaPrivateKey = string.Empty;
        private bool _captchaEnabled = false;
        private int _captchaSeconds = 30;
        private int _captchaAttempts = 5;
        private bool _captchaUserAndIPAsCacheKey = true;

        public LoginController(IUsuariosBusiness usuarios, IEmpresasBusiness empresasBusiness, IPermisosBusiness permisosBusiness, IMemoryCache memoryCache)
        {
            _usuarios = usuarios;
            _empresasBusiness = empresasBusiness;
            _permisosBusiness = permisosBusiness;

            _captchaSiteKey = "Captcha:SiteKey".FromAppSettings<string>(notFoundException: true);
            _captchaPrivateKey = "Captcha:PrivateKey".FromAppSettings<string>(notFoundException: true);
            _captchaEnabled = "Captcha:Enabled".FromAppSettings<bool>(notFoundException: true);
            _captchaSeconds = "Captcha:Seconds".FromAppSettings<int>(notFoundException: true);
            _captchaAttempts = "Captcha:Attempts".FromAppSettings<int>(notFoundException: true);
            _captchaUserAndIPAsCacheKey = "Captcha:UserAndIPAsCacheKey".FromAppSettings<bool>(notFoundException: true);
            this.memoryCache = memoryCache;
        }

        public async Task<ActionResult> Index()
        {
            var empresas = await _empresasBusiness.GetAllByIdUsuario(_permisosBusiness.User?.Id ?? 0);
            var model = new LoginViewModel
            {
                MaestroEmpresas = empresas
                    .ToDropDownList(e => e.IdEmpresa, e => e.RazonSocial, descriptionIncludesKey: false)
            };

            model.SiteKey = _captchaSiteKey;

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> SignIn(LoginViewModel model)
        {
            var doCaptcha = false;
            var validCaptcha = false;
            try
            {
                await ExceedAttemptsByUser(model.User);

                if (model.User.IsNull() || model.Pass.IsNull())
                    throw new Exception("Usuario y contraseña son obligatorios");

                doCaptcha = SessionHelper.DoCaptcha(HttpContext.Session);
                if (doCaptcha)
                {
                    validCaptcha = false;

                    if (Request.Form.ContainsKey("g-recaptcha-response"))
                    {
                        var captcha = Request.Form["g-recaptcha-response"].ToString();

                        validCaptcha = await ReCaptchaHelper.ValidarCaptchaV2(captcha);
                    }

                    //if (!validCaptcha)
                    //{
                    //    //return View("Index", model);
                    //    //throw new Exception("Captcha incorrecto");
                    //    throw new RequiredCaptchaException();
                    //}
                }

                await _permisosBusiness.SignIn(HttpContext, model);

                return Json(() => Url.Action("Index", "Home"));
            }
            catch (LoginFailedException ex)
            {
                if (doCaptcha)
                    return Json(new RequiredCaptchaException(ex.Message));
                else
                    return Json(ex);
            }
            catch (PasswordExpiredException ex)
            {
                await ClearAttemptsByUser(model.User);

                return Json(ex.Code, Url.Action("ResetPassword", "Login"));
            }
            catch (UserNotFoundException ex)
            {
                if (doCaptcha)
                    return Json(new RequiredCaptchaException(ex.Message));
                else
                    return Json(ex);
            }
            catch (PermissionException ex)
            {
                if (doCaptcha)
                    return Json(new RequiredCaptchaException(ex.Message));
                else
                    return Json(ex);
            }
            catch (EmpresaNotDefinedException ex)
            {
                await ClearAttemptsByUser(model.User);

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
        public async Task<JsonResult> ResetPassword(LoginViewModel model)
        {
            try
            {
                await _usuarios.ResetPassword(HttpContext, model.User, model.Pass, model.NewPass, model.RepeatPass);

                return Json(() => Url.Action("Index", "Home"));
            }
            catch (WeekPasswordException ex)
            {
                return Json(ex);
            }
            catch (PasswordNotMatchException ex)
            {
                return Json(ex);
            }
            catch (UserNotFoundException ex)
            {
                return Json(ex);
            }
            catch (TooManyRecordsException ex)
            {
                return Json(ex);
            }
            catch (WrongPasswordException ex)
            {
                return Json(ex);
            }
            catch (Exception ex)
            {
                return Json(0, ex.Message);
            }
        }

        public new async Task<ActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [HttpPost]
        public async Task<JsonResult> GetEmpresasUsuarios([FromForm] string user)
        {
            try
            {
                var empresas = (await _empresasBusiness.GetAllByIdUsuario(_permisosBusiness.UserSession(user.ToLower())?.Id ?? 0))
                    .Select(e => new { e.IdEmpresa, e.RazonSocial })
                    .ToList();

                return Json(() => empresas);
            }
            catch (UserNotFoundException ex)
            {
                return Json(ex);
            }
            catch (Exception ex)
            {
                return Json(0, ex.Message);
            }
        }

        private async Task<bool> ExceedAttemptsByUser(string usuario)
        {
            bool bReturn = false;

            if (!_captchaEnabled)
                return false;

            var ip = HttpContext.Request.GetIP();
            var cacheKey = string.Empty;

            if (_captchaUserAndIPAsCacheKey)
                cacheKey = usuario + "_" + ip;
            else
                cacheKey = ip;

            LoginAttempt loginAttemp;
            memoryCache.TryGetValue(cacheKey, out loginAttemp);
            if (loginAttemp != null)
            {
                loginAttemp.Intento++;

                if (loginAttemp.Intento >= _captchaAttempts)
                {
                    bReturn = true;

                    SessionHelper.SetObjectAsString(HttpContext.Session, SessionHelper.SessionKey.DO_CAPTCHA.Description(), "True");
                }

                memoryCache.Set(cacheKey, loginAttemp); //, DateTime.Now.AddSeconds(_captchaSeconds));
            }
            else
            {
                loginAttemp = new LoginAttempt { Usuario = usuario, Ip = ip, Intento = 1 };
                //memoryCache.Set(cacheKey, loginAttemp, DateTime.Now.AddSeconds(_captchaSeconds));
                memoryCache.Set(cacheKey, loginAttemp);
            }

            return bReturn;
        }

        private async Task ClearAttemptsByUser(string usuario)
        {
            if (!_captchaEnabled)
                return;

            var ip = HttpContext.Request.GetIP();
            var cacheKey = string.Empty;

            if (_captchaUserAndIPAsCacheKey)
                cacheKey = usuario + "_" + ip;
            else
                cacheKey = ip;

            memoryCache.Remove(cacheKey);
            SessionHelper.SetObjectAsString(HttpContext.Session, SessionHelper.SessionKey.DO_CAPTCHA.Description(), string.Empty);
        }
    }
}