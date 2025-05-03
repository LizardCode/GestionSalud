using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IAfipAuthRepository
    {
        Task<IList<TAfipAuth>> GetAll<TAfipAuth>(IDbTransaction transaction = null);

        Task<TAfipAuth> GetById<TAfipAuth>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TAfipAuth>(TAfipAuth entity, IDbTransaction transaction = null);

        Task<bool> Update<TAfipAuth>(TAfipAuth entity, IDbTransaction transaction = null);

        Task<Domain.Entities.AfipAuth> GetValidSignToken(int idEmpresa, string servicio);

        Task<Domain.Entities.AfipAuth> NewSignToken(int idEmpresa, string crt, string pk, string cuit, string servicio, bool useProd);

    }
}