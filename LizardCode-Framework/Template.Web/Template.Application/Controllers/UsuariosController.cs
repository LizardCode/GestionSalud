using Dapper.DataTables.Models;
using LizardCode.Framework.Aplication.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Template.Application.Interfaces.Business;
using Template.Application.Models.Usuarios;

namespace Template.Application.Controllers
{
    public class UsuariosController : BusinessController
    {
        private readonly IUsuariosBusiness _users;
        private readonly ILookupsBusiness _lookups;


        public UsuariosController(IUsuariosBusiness users, ILookupsBusiness lookups)
        {
            _users = users;
            _lookups = lookups;
        }


        [Authorize(Roles = "ADMIN")]
        public ActionResult Index()
        {
            var usersType = new[]
            {
                new { IdTipoUsuario = 1, Descripcion = "Administrador"},
                new { IdTipoUsuario = 2, Descripcion = "Usuario"}
            };

            var model = new UsuarioViewModel
            {
                MaestroTipoUsuarios = new SelectList(usersType, "IdTipoUsuario", "Descripcion")
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
    }
}