using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IFinanciadoresPadronRepository
    {
        Task<IList<TFinanciadorPadron>> GetAll<TFinanciadorPadron>(IDbTransaction transaction = null);

        Task<TFinanciadorPadron> GetById<TFinanciadorPadron>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TFinanciadorPadron>(TFinanciadorPadron entity, IDbTransaction transaction = null);

        Task<bool> Update<TFinanciadorPadron>(TFinanciadorPadron entity, IDbTransaction transaction = null);

        Task<FinanciadorPadron> GetByIdFinanciadorAndDocumento(long idFinanciador, string documento, IDbTransaction transaction = null);
        Task<bool> RemoveById(long idFinanciadorPadron, IDbTransaction transaction = null);
        Task<bool> RemoveByIdFinanciador(long idFinanciador, IDbTransaction transaction = null);
        DataTablesCustomQuery GetAllCustomQuery();
    }
}
