using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.CierreMes;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class CierreMesController : BusinessController
    {
        private readonly ICierreMesBusiness _cierreMesBusiness;

        public CierreMesController(ICierreMesBusiness cierreMesBusiness)
        {
            _cierreMesBusiness = cierreMesBusiness;
        }

        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<ActionResult> Index()
        {
            var ejercicios = (await _lookupsBusiness.GetEjerciciosByIdEmpresa(_permisosBusiness.User.IdEmpresa))
                .Select(e => new
                {
                    IdEjercicio = e.IdEjercicio,
                    Descripcion = string.Concat(e.Codigo, " - ", e.FechaInicio.ToString("dd/MM/yyyy"), " a ", e.FechaFin.ToString("dd/MM/yyyy"))
                })
                .ToList();

            var model = new CierreMesViewModel
            {
                MaestroEjercicios = new SelectList(ejercicios, "IdEjercicio", "Descripcion", ejercicios.Last().IdEjercicio),
            };

            return ActivateMenuItem(model: model);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Detalle(int id)
        {
            var cierres = await _cierreMesBusiness.GetDetalle(id);
            return Json(() => cierres);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN, ADMINISTRACION")]
        public async Task<JsonResult> Cierre(int idEjercicio, int anno, int mes, string modulo)
        {
            var cierre = await _cierreMesBusiness.CierreMes(idEjercicio, anno, mes, modulo);
            return Json(() => cierre);
        }

    }
}
