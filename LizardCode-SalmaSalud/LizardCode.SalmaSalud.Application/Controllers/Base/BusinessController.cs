using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using System;
using System.Linq;
using System.Reflection;
using LizardCode.Framework.Application.Controllers.Base;

namespace LizardCode.SalmaSalud.Application.Controllers.Base
{
    public partial class BusinessController : BaseController
    {
        protected readonly IPermisosBusiness _permisosBusiness;
        protected readonly IImpresionesBusiness _impresionesBusiness;
        protected readonly ILookupsBusiness _lookupsBusiness;


        public BusinessController()
        {
            _permisosBusiness = this.GetService<IPermisosBusiness>();
            _impresionesBusiness = this.GetService<IImpresionesBusiness>();
            _lookupsBusiness = this.GetService<ILookupsBusiness>();
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            DetectLoggedUser(context);
        }

        public ActionResult ActivateMenuItem(string view = null, object model = null, string menuItem = null)
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            TempData.Remove("MenuItem");
            if (!string.IsNullOrEmpty(menuItem))
                TempData.Add("MenuItem", menuItem);
            else
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
                ViewBag.Empresa = _permisosBusiness.User.Empresa ?? throw new Exception("Sin Empresa");
                ViewBag.CUIT = _permisosBusiness.User.CUIT ?? throw new Exception("Sin Empresa");
            }
            catch
            {
                context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).GetAwaiter().GetResult();
                context.Result = RedirectToAction("Index", "Login");
            }
        }
    }
}