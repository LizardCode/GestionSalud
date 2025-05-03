using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ILiquidacionesProfesionalesPrestacionesRepository
    {
        Task<IList<TLiquidacionProfesionalPrestacion>> GetAll<TLiquidacionProfesionalPrestacion>(IDbTransaction transaction = null);

        Task<TLiquidacionProfesionalPrestacion> GetById<TLiquidacionProfesionalPrestacion>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TLiquidacionProfesionalPrestacion>(TLiquidacionProfesionalPrestacion entity, IDbTransaction transaction = null);

        Task<bool> Update<TLiquidacionProfesionalPrestacion>(TLiquidacionProfesionalPrestacion entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdLiquidacionProfesional(long idLiquidacionProfesional, IDbTransaction transaction = null);

        Task<IList<LiquidacionProfesionalPrestacion>> GetAllByIdLiquidacionProfesional(long idLiquidacionProfesional, IDbTransaction transaction = null);

        Task<bool> RemoveById(long idLiquidacionProfesionalPrestacion, IDbTransaction transaction = null);
    }
}
