using Dapper.DataTables.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IDiasRepository
    {
        Task<IList<TDia>> GetAll<TDia>(IDbTransaction transaction = null);

        Task<TDia> GetById<TDia>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TDia>(TDia entity, IDbTransaction transaction = null);

        Task<bool> Update<TDia>(TDia entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
    }
}
