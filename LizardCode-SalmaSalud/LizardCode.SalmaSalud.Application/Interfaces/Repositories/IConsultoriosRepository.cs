using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IConsultoriosRepository
    {
        Task<IList<TConsultorio>> GetAll<TConsultorio>(IDbTransaction transaction = null);

        Task<TConsultorio> GetById<TConsultorio>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TConsultorio>(TConsultorio entity, IDbTransaction transaction = null);

        Task<bool> Update<TConsultorio>(TConsultorio entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<List<Consultorio>> GetAllByIdEmpresa(int idEmpresa);
    }
}
