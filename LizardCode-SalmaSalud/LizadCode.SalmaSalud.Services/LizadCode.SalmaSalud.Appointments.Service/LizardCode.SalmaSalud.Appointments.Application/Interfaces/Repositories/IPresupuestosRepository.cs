using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Appointments.Application.Interfaces.Repositories
{
    public interface IPresupuestosRepository
    {
        Task<TPresupuesto> GetById<TPresupuesto>(int id, IDbTransaction transaction = null);
        Task<IList<Domain.Entities.Presupuesto>> GetPresupuestosAVencer();
        Task<long> Insert<TPresupuesto>(TPresupuesto entity, IDbTransaction transaction = null);
        Task<bool> Update<TPresupuesto>(TPresupuesto entity, IDbTransaction transaction = null);
    }
}
