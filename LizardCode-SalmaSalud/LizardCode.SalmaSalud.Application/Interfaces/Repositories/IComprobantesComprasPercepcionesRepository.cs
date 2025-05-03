using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesComprasPercepcionesRepository
    {
        Task<IList<ComprobanteCompraPercepcion>> GetAllByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null);

        Task<bool> Insert(ComprobanteCompraPercepcion entity, IDbTransaction transaction = null);

        Task<bool> Update(ComprobanteCompraPercepcion entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null);

        Task<bool> DeleteByIdComprobanteCompraAndCuenta(int idComprobanteCompra, int idCuentaContable, IDbTransaction transaction = null);

        Task<ComprobanteCompraPercepcion> GetByIdComprobanteCompraAndCuenta(int idComprobanteCompra, int idCuentaContable, IDbTransaction transaction = null);

        Task<bool> RemoveAllByIdSdoCtaCtePrv(int idSdoCtaCtePrv, IDbTransaction transaction = null);

    }
}