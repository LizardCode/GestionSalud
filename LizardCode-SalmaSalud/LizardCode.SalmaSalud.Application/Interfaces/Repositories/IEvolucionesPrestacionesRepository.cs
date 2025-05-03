using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IEvolucionesPrestacionesRepository
    {
        Task<IList<TEvolucionPrestacion>> GetAll<TEvolucionPrestacion>(IDbTransaction transaction = null);

        Task<TEvolucionPrestacion> GetById<TEvolucionPrestacion>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TEvolucionPrestacion>(TEvolucionPrestacion entity, IDbTransaction transaction = null);

        Task<bool> Update<TEvolucionPrestacion>(TEvolucionPrestacion entity, IDbTransaction transaction = null);

        Task<IList<EvolucionPrestacion>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);

        Task<bool> DeleteByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);

        Task<List<Custom.PrestacionFinanciador>> GetPrestacionesFinanciador(Dictionary<string, object> filters);

        Task<List<Custom.EvolucionPrestacion>> GetPrestacionesALiquidar(DateTime desde, DateTime hasta, int idProfesional, IDbTransaction transaction = null);
    }
}
