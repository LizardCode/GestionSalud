using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IEvolucionesOrdenesRepository
    {
        Task<IList<TEvolucionOrden>> GetAll<TEvolucionOrden>(IDbTransaction transaction = null);

        Task<TEvolucionOrden> GetById<TEvolucionOrden>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TEvolucionOrden>(TEvolucionOrden entity, IDbTransaction transaction = null);

        Task<bool> Update<TEvolucionOrden>(TEvolucionOrden entity, IDbTransaction transaction = null);

        Task<IList<EvolucionOrden>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);

        Task<bool> DeleteByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);
    }
}