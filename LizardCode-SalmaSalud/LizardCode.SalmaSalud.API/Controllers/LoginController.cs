using LizardCode.SalmaSalud.API.Infrastructure.JWT;
using LizardCode.SalmaSalud.API.Models;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using Microsoft.AspNetCore.Mvc;

namespace LizardCode.SalmaSalud.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : Controller
    {
        IConfiguration _configuration;
        IPermisosBusiness _permisosBusiness;

        public LoginController(IConfiguration configuration, IPermisosBusiness permisosBusiness) 
        {
            this._configuration = configuration;
            this._permisosBusiness = permisosBusiness;
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginModel model)
        {
            Domain.Entities.Usuario usuario = null;
            try 
            { 
                usuario = await _permisosBusiness.ValidateLogin(new Application.Models.Login.LoginViewModel
                {
                    User = model.User,
                    Pass = model.Password
                });

                if (usuario == null)
                    return Unauthorized();
            }
            catch (Exception ex) 
            {
                return Unauthorized(ex.Message);
            }
            
            var tokenResponse = new TokenProvider(_configuration).Create(usuario.IdUsuario);

            return Json(tokenResponse);
        }
    }
}
