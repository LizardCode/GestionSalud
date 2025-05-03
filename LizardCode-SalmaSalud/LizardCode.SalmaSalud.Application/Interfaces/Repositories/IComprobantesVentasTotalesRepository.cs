using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesVentasTotalesRepository
    {
        Task<IList<ComprobanteVentaTotales>> GetAllByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null);

        Task<bool> Insert(ComprobanteVentaTotales entity, IDbTransaction transaction = null);

        Task<bool> Update(ComprobanteVentaTotales entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdComprobanteVenta(int idComprobanteVenta, IDbTransaction transaction = null);

        Task<bool> DeleteByIdComprobanteVentaAndAlicuota(int idComprobanteVenta, int item, IDbTransaction transaction = null);

        Task<ComprobanteVentaTotales> GetByIdComprobanteVentaAndAlicuota(int idComprobanteVenta, int item, IDbTransaction transaction = null);
        Task<bool> RemoveAllByIdSdoCtaCteCli(int idSdoCtaCteCli, IDbTransaction transaction = null);
    }
}