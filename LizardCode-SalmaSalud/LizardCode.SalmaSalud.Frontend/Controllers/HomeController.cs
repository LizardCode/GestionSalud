using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Enums;
using LizardCode.SalmaSalud.Controllers.Base;
using LizardCode.SalmaSalud.Application.Startup;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Collections.Generic;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Models.TurnosSolicitud;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.SalmaSalud.Application.Models.Home;

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
        public async Task<ActionResult> Index()
        {
            TempData.Remove("MenuItem");
            TempData.Add("MenuItem", Url.Action("Index", "Home"));

            var especialidades = (await _lookupsBusiness.GetAllEspecialidades()).ToList();
            especialidades.Insert(0, new Domain.Entities.Especialidades { IdEspecialidad = 0, Descripcion = "TODAS" });
            var model = new HomeViewModel
            {                
                MaestroEspecialidades = especialidades
                    .ToDropDownList(k => k.IdEspecialidad, t => t.Descripcion)
            };

            if (base._permisosBusiness.User.IdTipoUsuario == (int)TipoUsuario.Administrador)
            {
                return View("Administrador", model);
            }
            else if (base._permisosBusiness.User.IdTipoUsuario == (int)TipoUsuario.Administracion)
            {
                return View("Administracion");
            }
            else if (base._permisosBusiness.User.IdTipoUsuario == (int)TipoUsuario.Recepcion)
            {
                return View("Recepcion", model);
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