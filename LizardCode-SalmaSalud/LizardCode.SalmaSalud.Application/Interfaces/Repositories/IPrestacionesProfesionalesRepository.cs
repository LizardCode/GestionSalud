using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPrestacionesProfesionalesRepository    
    {
        Task<IList<TPrestacionProfesional>> GetAll<TPrestacionProfesional>(IDbTransaction transaction = null);

        Task<TPrestacionProfesional> GetById<TPrestacionProfesional>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TPrestacionProfesional>(TPrestacionProfesional entity, IDbTransaction transaction = null);

        Task<bool> Update<TPrestacionProfesional>(TPrestacionProfesional entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdPrestacion(long idPrestacion, IDbTransaction transaction = null);

        Task<IList<PrestacionProfesional>> GetAllByIdPrestacion(long idPrestacion, IDbTransaction transaction = null);
        Task<PrestacionProfesional> GetByIdPrestacionAndProfesional(int idPrestacion, int idProfesional, IDbTransaction transaction = null);
    }
}
