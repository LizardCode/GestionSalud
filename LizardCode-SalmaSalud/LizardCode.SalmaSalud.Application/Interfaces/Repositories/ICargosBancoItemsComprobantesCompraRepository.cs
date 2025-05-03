using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICargosBancoItemsComprobantesCompraRepository
    {
        Task<bool> DeleteByIdCargoBancoItem(int idCargoBanco, int item, IDbTransaction transaction = null);

        Task<bool> DeleteByIdCargoBanco(int idCargoBanco, IDbTransaction transaction = null);

        Task<CargoBancoItemComprobanteCompra> GetByIdCargoBancoItem(int idCargoBanco, int item, IDbTransaction transaction = null);

        Task<List<CargoBancoItemComprobanteCompra>> GetByIdCargoBanco(int idCargoBanco, IDbTransaction transaction = null);

        Task<bool> Insert(CargoBancoItemComprobanteCompra entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdComprobanteCompraItem(int idComprobanteCompra, int item, IDbTransaction transaction = null);

    }
}