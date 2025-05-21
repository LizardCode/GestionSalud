using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Startup;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using System.Runtime.InteropServices;

namespace LizardCode.SalmaSalud.Controllers
{
    public class HomeController : BusinessController
    {
        private readonly IPacientesBusiness _pacientesBusiness;
        private readonly IFinanciadoresBusiness _financiadoresBusiness;

        public HomeController(IPacientesBusiness pacientesBusiness, IFinanciadoresBusiness financiadoresBusiness)
        {
            _pacientesBusiness = pacientesBusiness;
            _financiadoresBusiness = financiadoresBusiness;
        }

        [Authorize]
        public ActionResult Index()
        {
            TempData.Remove("MenuItem");
            TempData.Add("MenuItem", Url.Action("Index", "Home"));

            if (base._permisosBusiness.User.IdTipoUsuario == (int)TipoUsuario.Administrador)
            {
                return View("Administrador");
            }
            else if (base._permisosBusiness.User.IdTipoUsuario == (int)TipoUsuario.Administracion)
            {
                return View("Administracion");
            }
            else if (base._permisosBusiness.User.IdTipoUsuario == (int)TipoUsuario.Recepcion)
            {
                return View("Recepcion");
            }
            else if (base._permisosBusiness.User.IdTipoUsuario == (int)TipoUsuario.Profesional)
            {
                return View("Profesional");
            }
            else if (base._permisosBusiness.User.IdTipoUsuario == (int)TipoUsuario.ProfesionalExterno)
            {
                return View("ProfesionalExterno");
            }
            else if (base._permisosBusiness.User.IdTipoUsuario == (int)TipoUsuario.Paciente)
            {
                return RedirectToAction("Home", "PortalPacientes");
                //return View("Paciente");
            }
            else
            {
                return View(GlobalSettings.PaginaInicial);
            }
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ADMIN_CreateClientes()
        {
            await _pacientesBusiness.ADMIN_CreateClientes();

            return Json(true);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ADMIN_CreateUsuarios()
        {
            await _pacientesBusiness.ADMIN_CreateUsuarios();

            return Json(true);
        }
    }
}