using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.Framework.Application.Interfaces.Business;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LizardCode.Framework.Application.Controllers.Base
{
    public partial class BusinessController : BaseController
    {
        private readonly IPermisosBusiness _permissions;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;


        public BusinessController()
        {
            _permissions = this.GetService<IPermisosBusiness>();
            _actionDescriptorCollectionProvider = this.GetService<IActionDescriptorCollectionProvider>();
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            DetectLoggedUser(context);
        }

        public ActionResult ActivateMenuItem(string view = null, object model = null)
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            TempData.Remove("MenuItem");
            TempData.Add("MenuItem", Url.Action(action, controller));

            return View(viewName: view, model: model);
        }

        private void DetectLoggedUser(ActionExecutingContext context)
        {
            var controllerName = ControllerContext.ActionDescriptor.ControllerName;
            var actionName = ControllerContext.ActionDescriptor.ActionName;
            var actionDescriptor = _actionDescriptorCollectionProvider.ActionDescriptors
                .Items
                .OfType<ControllerActionDescriptor>()
                .FirstOrDefault(f => f.ControllerName.Equals(controllerName, StringComparison.InvariantCultureIgnoreCase));

            if (actionDescriptor != null)
            {
                var controllerAuthorize = actionDescriptor.ControllerTypeInfo.GetCustomAttribute(typeof(AuthorizeAttribute));

                if (controllerAuthorize == null)
                {
                    var action = actionDescriptor.ControllerTypeInfo
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
                ViewBag.UsuarioSesion = _permissions.User ?? throw new Exception();
            }
            catch
            {
                context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).GetAwaiter().GetResult();
                context.Result = RedirectToAction("Index", "Login");
            }
        }

        /// <summary>
        /// Gets the usuario login.
        /// </summary>
        /// <returns></returns>
        public IPermisoUsuario GetUsuarioLogin()
        {
            return (IPermisoUsuario)ViewBag.UsuarioSesion;
        }
    }
}