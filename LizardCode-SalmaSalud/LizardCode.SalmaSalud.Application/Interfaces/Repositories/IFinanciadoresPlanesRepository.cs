using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IFinanciadoresPlanesRepository
    {
        Task<IList<TFinanciadorPlan>> GetAll<TFinanciadorPlan>(IDbTransaction transaction = null);

        Task<TFinanciadorPlan> GetById<TFinanciadorPlan>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TFinanciadorPlan>(TFinanciadorPlan entity, IDbTransaction transaction = null);

        Task<bool> Update<TFinanciadorPlan>(TFinanciadorPlan entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdFinanciador(long idFinanciador, IDbTransaction transaction = null);

        Task<IList<FinanciadorPlan>> GetAllByIdFinanciador(long idFinanciador, IDbTransaction transaction = null);
        Task<bool> RemoveById(long idFinanciadorPlan, IDbTransaction transaction = null);

        Task<bool> ExistePacienteAsociado(long idFinanciadorPlan, IDbTransaction transaction = null);
        Task<FinanciadorPlan> GetByCodigo(string codigo, IDbTransaction transaction = null);
    }
}
