using LizardCode.Framework.Aplication.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Startup;

namespace Template.Application.Controllers
{
    public class HomeController : BusinessController
    {
        [Authorize]
        public ActionResult Index()
        {
            return View(GlobalSettings.PaginaInicial);
        }
    }
}