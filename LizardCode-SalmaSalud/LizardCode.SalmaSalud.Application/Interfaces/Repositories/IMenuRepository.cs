using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IMenuRepository
    {
        Task<IList<Custom.Menu>> GetAllCustom();
        Task<TMenu> GetById<TMenu>(int id);
        Task<IList<Menu>> GetByTipoUsuarioCustom(int idTipoUsuario);
    }
}