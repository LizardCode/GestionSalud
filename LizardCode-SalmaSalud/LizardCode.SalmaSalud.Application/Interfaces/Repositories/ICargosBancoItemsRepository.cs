using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICargosBancoItemsRepository
    {
        Task<IList<Domain.EntitiesCustom.CargoBancoItem>> GetAllByIdCargoBanco(int idCargoBanco, IDbTransaction transaction = null);

        Task<bool> Insert(CargoBancoItem entity, IDbTransaction transaction = null);

        Task<bool> Update(CargoBancoItem entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdCargoBanco(int idCargoBanco, IDbTransaction transaction = null);

        Task<bool> DeleteByIdCargoBancoAndItem(int idCargoBanco, int item, IDbTransaction transaction = null);

        Task<CargoBancoItem> GetByIdCargoBancoAndItem(int idCargoBanco, int item, IDbTransaction transaction = null);
    }
}