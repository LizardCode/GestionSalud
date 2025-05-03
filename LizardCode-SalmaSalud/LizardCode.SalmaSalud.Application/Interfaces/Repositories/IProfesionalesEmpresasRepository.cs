using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IProfesionalesEmpresasRepository
    {
        Task<IList<TProfesionalEmpresa>> GetAll<TProfesionalEmpresa>(IDbTransaction transaction = null);

        Task<TProfesionalEmpresa> GetById<TProfesionalEmpresa>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TProfesionalEmpresa>(TProfesionalEmpresa entity, IDbTransaction transaction = null);

        Task<bool> Update<TProfesionalEmpresa>(TProfesionalEmpresa entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdProfesional(int idProfesional, IDbTransaction transaction = null);

        Task<IList<ProfesionalEmpresa>> GetAllByIdProfesional(int idProfesional, IDbTransaction transaction = null);
    }
}