using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class PlanCuentasController : BusinessController
    {
        private readonly IPlanCuentasBusiness _planCuentasBusiness;

        public PlanCuentasController(IPlanCuentasBusiness planCuentasBusiness)
        {
            _planCuentasBusiness = planCuentasBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var model = await _planCuentasBusiness.Get();
            return ActivateMenuItem(model: model);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Detalle(int id)
        {
            var cuenta = await _planCuentasBusiness.GetCustom(id);
            return Json(() => cuenta);
        }
    }
}
