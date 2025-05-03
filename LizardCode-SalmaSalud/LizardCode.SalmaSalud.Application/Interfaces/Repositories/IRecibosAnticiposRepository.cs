using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IRecibosAnticiposRepository
    {
        Task<IList<TReciboAnticipo>> GetAll<TReciboAnticipo>(IDbTransaction transaction = null);

        Task<TReciboAnticipo> GetById<TReciboAnticipo>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TReciboAnticipo>(TReciboAnticipo entity, IDbTransaction transaction = null);

        Task<bool> Update<TReciboAnticipo>(TReciboAnticipo entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdRecibo(int idRecibo, IDbTransaction transaction = null);

        Task<IList<Domain.EntitiesCustom.ReciboAnticipo>> GetAnticiposImputar(int idCliente, string idMoneda, int idEmpresa, IDbTransaction transaction = null);

        Task<IList<Domain.EntitiesCustom.ReciboAnticipo>> GetByIdRecibo(int idRecibo, int idCliente);

        Task<ReciboAnticipo> GetByIdRecibo(int idRecibo, IDbTransaction transaction = null);

        Task<ReciboAnticipo> GetByIdAnticipo(int idAnticipo, IDbTransaction transaction = null);
    }
}