using LizardCode.SalmaSalud.Application.Models.Menu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IMenuBusiness
    {
        Task<List<ItemMenu>> GetAll();
        Task<List<ItemMenu>> GetAllByTipoUsuario(int idTipoUsuario);
    }
}