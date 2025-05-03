using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IOrdenesPagoAnticiposRepository
    {
        Task<IList<TOrdenPagoAnticipo>> GetAll<TOrdenPagoAnticipo>(IDbTransaction transaction = null);

        Task<TOrdenPagoAnticipo> GetById<TOrdenPagoAnticipo>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TOrdenPagoAnticipo>(TOrdenPagoAnticipo entity, IDbTransaction transaction = null);

        Task<bool> Update<TOrdenPagoAnticipo>(TOrdenPagoAnticipo entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null);

        Task<IList<Custom.OrdenPagoAnticipo>> GetAnticiposImputar(int idProveedor, string idMoneda, int idEmpresa, IDbTransaction transaction = null);

        Task<IList<Custom.OrdenPagoAnticipo>> GetByIdOrdenPago(int idOrdenPago, int idProveedor, IDbTransaction transaction = null);

        Task<IList<OrdenPagoAnticipo>> GetAllByIdOrdenPago(int idOrdenPago, IDbTransaction transaction = null);

        Task<OrdenPagoAnticipo> GetByIdAnticipo(int idAnticipo, IDbTransaction transaction = null);

    }
}