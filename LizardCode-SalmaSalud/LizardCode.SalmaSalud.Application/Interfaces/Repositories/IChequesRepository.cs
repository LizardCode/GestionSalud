using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IChequesRepository
    {
        Task<IList<TCheque>> GetAll<TCheque>(IDbTransaction transaction = null);

        Task<TCheque> GetById<TCheque>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCheque>(TCheque entity, IDbTransaction transaction = null);

        Task<bool> Update<TCheque>(TCheque entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<bool> ValidarNumeroCheque(int idBanco, int idTipoCheque, string nroDesde, string nroHasta);

        Task<Cheque> GetByNumeroCheque(string numeroCheque, int idBanco, int idTipoCheque, int idEmpresa, IDbTransaction transaction = null);

        Task<List<Domain.EntitiesCustom.ChequeTerceros>> GetChequesCartera(int idEmpresa, string term);

        Task<Cheque> GetPrimerChequeDisponible(int idBanco, int idTipoCheque, int idEmpresa);

        Task<bool> ValidarNumeroChequeDisponible(int idBanco, int idTipoCheque, string nroCheque, int idEmpresa);

        Task<List<Domain.EntitiesCustom.Cheque>> GetChequesADebitar(int idEmpresa, int idBanco);

        Task<bool> DeleteByIdCheque(int idCheque, IDbTransaction transaction = null);

        Task<bool> GetVerificarDuplicados(int idEmpresa, int idBanco, int idTipoCheque, string nroDesde, string nroHasta);
    }
}