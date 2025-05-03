using LizardCode.Framework.Aplication.Common.Exceptions;
using LizardCode.Framework.Aplication.Controllers.Base;
using LizardCode.Framework.Aplication.Interfaces.Business;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Template.Application.Interfaces.Business;
using Template.Application.Models.Login;

namespace Template.Application.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IUsuariosBusiness _usuarios;
        private readonly IPermisosBusiness _permisos;


        public LoginController(IUsuariosBusiness usuarios, IPermisosBusiness permisos)
        {
            _usuarios = usuarios;
            _permisos = permisos;
        }


        public ActionResult Index()
        {
            var model = new LoginViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> SignIn(LoginViewModel model)
        {
            try
            {
                if (model.User.IsNull() || model.Pass.IsNull())
                    throw new Exception("Usuario y contraseña son obligatorios");

                await _permisos.SignIn(HttpContext, model.User, model.Pass);

                return Json(() => Url.Action("Index", "Home"));
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
    }
}