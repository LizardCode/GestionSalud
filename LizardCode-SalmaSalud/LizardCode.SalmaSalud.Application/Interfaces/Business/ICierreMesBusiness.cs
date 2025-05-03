using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ICierreMesBusiness
    {
        Task<List<Custom.CierreMes>> GetDetalle(int id);
        Task<bool> CierreMes(int idEjercicio, int anno, int mes, string modulo);
    }
}