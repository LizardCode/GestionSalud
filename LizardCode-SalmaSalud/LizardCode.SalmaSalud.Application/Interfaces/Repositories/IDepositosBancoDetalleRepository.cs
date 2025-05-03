using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IDepositosBancoDetalleRepository
    {
        Task<IList<TDepositoBancoDetalle>> GetAll<TDepositoBancoDetalle>(IDbTransaction transaction = null);

        Task<TDepositoBancoDetalle> GetById<TDepositoBancoDetalle>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TDepositoBancoDetalle>(TDepositoBancoDetalle entity, IDbTransaction transaction = null);

        Task<bool> Update<TDepositoBancoDetalle>(TDepositoBancoDetalle entity, IDbTransaction transaction = null);

        Task<List<Custom.DepositoBancoDetalle>> GetAllByIdDepositoBanco(int idDepositoBanco, IDbTransaction transaction = null);

        Task<bool> DeleteByIdDepositoBanco(int idDepositoBanco, IDbTransaction transaction = null);
    }
}