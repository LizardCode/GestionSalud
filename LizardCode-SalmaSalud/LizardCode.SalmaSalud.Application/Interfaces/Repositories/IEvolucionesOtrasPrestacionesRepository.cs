using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IEvolucionesOtrasPrestacionesRepository
    {
        Task<IList<TEvolucionOtraPrestacion>> GetAll<TEvolucionOtraPrestacion>(IDbTransaction transaction = null);

        Task<TEvolucionOtraPrestacion> GetById<TEvolucionOtraPrestacion>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TEvolucionOtraPrestacion>(TEvolucionOtraPrestacion entity, IDbTransaction transaction = null);

        Task<bool> Update<TEvolucionOtraPrestacion>(TEvolucionOtraPrestacion entity, IDbTransaction transaction = null);

        Task<IList<EvolucionOtraPrestacion>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);

        Task<bool> DeleteByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);

        Task<List<Custom.PrestacionProfesional>> GetPrestacionesProfesional(Dictionary<string, object> filters);
        Task<IList<Custom.EvolucionOtraPrestacion>> GetPrestacionesALiquidar(DateTime desde, DateTime hasta, int idProfesional, IDbTransaction transaction = null);
    }
}
