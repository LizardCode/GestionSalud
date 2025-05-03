using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IFinanciadoresPrestacionesProfesionalesRepository
    {
        Task<IList<TFinanciadorPrestacionProfesional>> GetAll<TFinanciadorPrestacionProfesional>(IDbTransaction transaction = null);

        Task<TFinanciadorPrestacionProfesional> GetById<TFinanciadorPrestacionProfesional>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TFinanciadorPrestacionProfesional>(TFinanciadorPrestacionProfesional entity, IDbTransaction transaction = null);

        Task<bool> Update<TFinanciadorPrestacionProfesional>(TFinanciadorPrestacionProfesional entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdFinanciadorPrestacion(long idFinanciadorPrestacion, IDbTransaction transaction = null);

        Task<IList<FinanciadorPrestacionProfesional>> GetAllByIdFinanciadorPrestacion(long idFinanciadorPrestacion, IDbTransaction transaction = null);
        Task<FinanciadorPrestacionProfesional> GetByIdPrestacionAndProfesional(int idFinanciadorPrestacion, int idProfesional, IDbTransaction transaction = null);
    }
}
