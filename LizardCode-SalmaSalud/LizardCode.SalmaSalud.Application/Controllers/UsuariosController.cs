using Dapper.DataTables.Models;
using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.SalmaSalud.Application.Controllers.Base;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Models.Usuarios;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Controllers
{
    public class UsuariosController : BusinessController
    {
        private readonly IUsuariosBusiness _users;

        public UsuariosController(IUsuariosBusiness users)
        {
            _users = users;
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> Index()
        {
            var empresas = await _lookupsBusiness.GetAllEmpresasLookup();
            var profesionales = await _lookupsBusiness.GetAllProfesionales(_permisosBusiness.User.IdEmpresa);
            var pacientes = await _lookupsBusiness.GetAllPacientes();

            var usersType = new[]
            {
                new { IdTipoUsuario = 1, Descripcion = "Administrador"},
                //new { IdTipoUsuario = 2, Descripcion = "Administración"},
                //new { IdTipoUsuario = 3, Descripcion = "Cuentas"},
                //new { IdTipoUsuario = 4, Descripcion = "Tesorería"},
                //new { IdTipoUsuario = 5, Descripcion = "Cuentas por Pagar"},
                //new { IdTipoUsuario = 6, Descripcion = "Cuentas por Cobrar"},
                //new { IdTipoUsuario = 7, Descripcion = "Profesional"},
                new { IdTipoUsuario = 8, Descripcion = "Recepción"}
                //new { IdTipoUsuario = 9, Descripcion = "Paciente"},
                //new { IdTipoUsuario = 10, Descripcion = "Profesional Externo"}
            };

            var model = new UsuarioViewModel
            {
                MaestroEmpresas = new SelectList(empresas, "IdEmpresa", "RazonSocial"),
                MaestroTipoUsuarios = new SelectList(usersType, "IdTipoUsuario", "Descripcion"),
                MaestroProfesionales = new SelectList(profesionales, "IdProfesional", "Nombre"),
                MaestroPacientes = new SelectList(pacientes, "IdPaciente", "Nombre")
            };

            return ActivateMenuItem(model: model);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ObtenerTodos([FromForm] DataTablesRequest request)
        {
            var results = await _users.GetAll(request);
            return Json(results);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Nuevo(UsuarioViewModel model)
        {
            await _users.New(model);
            return Json(() => true);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Obtener(int id)
        {
            var usuario = await _users.Get(id);
            return Json(() => usuario);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Modificar(UsuarioViewModel model)
        {
            await _users.Update(model);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Eliminar(int id)
        {
            await _users.Remove(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> Blanqueo(int id)
        {
            await _users.Blank(id);
            return Json(() => true);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<JsonResult> ValidarLogin(string login)
        {
            var result = await _users.CheckLogin(login);
            return Json(result);
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> ObtenerTotalesDashboard()
        {
            var usuarios = await _lookupsBusiness.GetAllUsuariosByIdEmpresaLookup(_permisosBusiness.User.Id);
            return Json(() => new { cantidad = usuarios.Count });
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> ObtenerAuditoriaLoginDashboard([FromForm] DataTablesRequest request)
        {
            var results = await _users.GetAllAuditoriaLogin(request);
            return Json(results);
        }
    }
}