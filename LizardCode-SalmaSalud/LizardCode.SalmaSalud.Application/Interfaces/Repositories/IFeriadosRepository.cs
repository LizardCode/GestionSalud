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
    public interface IFeriadosRepository
    {
        Task<IList<TFeriado>> GetAll<TFeriado>(IDbTransaction transaction = null);

        Task<TFeriado> GetById<TFeriado>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TFeriado>(TFeriado entity, IDbTransaction transaction = null);

        Task<bool> Update<TFeriado>(TFeriado entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<List<Feriado>> GetAllByIdEmpresa(int idEmpresa);
    }
}
