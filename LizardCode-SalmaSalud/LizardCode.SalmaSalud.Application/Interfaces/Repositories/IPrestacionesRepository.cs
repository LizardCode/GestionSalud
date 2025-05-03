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
    public interface IPrestacionesRepository
    {
        Task<IList<TPrestacion>> GetAll<TPrestacion>(IDbTransaction transaction = null);

        Task<TPrestacion> GetById<TPrestacion>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TPrestacion>(TPrestacion entity, IDbTransaction transaction = null);

        Task<bool> Update<TPrestacion>(TPrestacion entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<Prestacion> GetByCodigo(string codigo, IDbTransaction transaction = null);
        Task<IList<Prestacion>> GetAllPrestaciones(IDbTransaction transaction = null);
    }
}
