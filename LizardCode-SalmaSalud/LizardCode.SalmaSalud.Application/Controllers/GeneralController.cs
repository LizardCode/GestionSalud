using LizardCode.Framework.Application.Models.ExcelReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Models.GenericMessages;
using LizardCode.SalmaSalud.Application.Models.Turnos;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class GeneralController : BusinessController
    {
        public ActionResult Restricted()
        {
            var model = new GenericMessageViewModel
            {
                Code = 401,
                Title = "Restricted access",
                Subtitle =
                    "Your user does not have the necessary permission to access the requested section.\r\n" +
                    "Please contact an administrator.",
                Button = "Back",
                Redirection = Url.Action("Index", "Home")

            };

            return View("GenericMessages", model);
        }

        [Authorize]
        public async Task<IActionResult> ProcesarExcelView(string controllerForm, string actionForm)
        {
            ViewData.Add("ajaxProcesarAction", Url.Action(actionForm, controllerForm));

            return View("/Views/Shared/ExcelReader/ExcelReader.cshtml", new ExcelReaderViewModel());
        }
    }
}
