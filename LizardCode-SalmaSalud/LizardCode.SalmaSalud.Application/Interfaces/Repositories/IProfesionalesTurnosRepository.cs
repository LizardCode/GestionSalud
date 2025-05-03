using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IProfesionalesTurnosRepository
    {
        Task<TProfesionalTurno> GetById<TProfesionalTurno>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TProfesionalTurno>(TProfesionalTurno entity, IDbTransaction transaction = null);

        Task<bool> Update<TProfesionalTurno>(TProfesionalTurno entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdProfesional(int idProfesional, IDbTransaction transaction = null);

        Task<IList<ProfesionalTurno>> GetAllByIdProfesionalAndIdEmpresa(DateTime desde, DateTime hasta, int idProfesional, int idEmpresa, IDbTransaction transaction = null);

        Task<IList<ProfesionalTurno>> GetByIdProfesionalAndIdEmpresaAndFecha(DateTime desde, int idProfesional, int idEmpresa, IDbTransaction transaction = null);
        Task<bool> RemoveById(int idProfesionalTurno, IDbTransaction transaction = null);

        Task<bool> RemoveAllByFecha(DateTime desde, DateTime hasta, int idProfesional, int idEmpresa, IDbTransaction transaction = null);
        Task<bool> ExistenTurnosAgendadosByFecha(DateTime desde, DateTime hasta, int idProfesional, int idEmpresa, IDbTransaction transaction = null);
    }
}
