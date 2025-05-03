using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPresupuestosOtrasPrestacionesRepository
    {
        Task<IList<TPresupuestoOtraPrestacion>> GetAll<TPresupuestoOtraPrestacion>(IDbTransaction transaction = null);

        Task<TPresupuestoOtraPrestacion> GetById<TPresupuestoOtraPrestacion>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TPresupuestoOtraPrestacion>(TPresupuestoOtraPrestacion entity, IDbTransaction transaction = null);

        Task<bool> Update<TPresupuestoOtraPrestacion>(TPresupuestoOtraPrestacion entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdPresupuesto(long idPresupuesto, IDbTransaction transaction = null);

        Task<IList<PresupuestoOtraPrestacion>> GetAllByIdPresupuesto(long idPresupuesto, IDbTransaction transaction = null);
        Task<bool> RemoveById(long idPresupuestoOtraPrestacion, IDbTransaction transaction = null);
        Task<PresupuestoOtraPrestacion> GetPresupuestoOtraPrestacionById(long idPresupuestoOtraPrestacion, IDbTransaction transaction = null);
    }
}