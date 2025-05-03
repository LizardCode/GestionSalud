using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPresupuestosPrestacionesRepository
    {
        Task<IList<TPresupuestoPrestacion>> GetAll<TPresupuestoPrestacion>(IDbTransaction transaction = null);

        Task<TPresupuestoPrestacion> GetById<TPresupuestoPrestacion>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TPresupuestoPrestacion>(TPresupuestoPrestacion entity, IDbTransaction transaction = null);

        Task<bool> Update<TPresupuestoPrestacion>(TPresupuestoPrestacion entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdPresupuesto(long idPresupuesto, IDbTransaction transaction = null);

        Task<IList<PresupuestoPrestacion>> GetAllByIdPresupuesto(long idPresupuesto, IDbTransaction transaction = null);
        Task<bool> RemoveById(long idPresupuestoPrestacion, IDbTransaction transaction = null);
        Task<PresupuestoPrestacion> GetPresupuestoPrestacionById(long idPresupuestoPrestacion, IDbTransaction transaction = null);
    }
}