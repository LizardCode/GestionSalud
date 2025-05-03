using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IEvolucionesItemsRepository
    {
        Task<IList<TEvolucionItem>> GetAll<TEvolucionItem>(IDbTransaction transaction = null);

        Task<TEvolucionItem> GetById<TEvolucionItem>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TEvolucionItem>(TEvolucionItem entity, IDbTransaction transaction = null);

        Task<bool> Update<TEvolucionItem>(TEvolucionItem entity, IDbTransaction transaction = null);

        Task<IList<EvolucionPrestacion>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);

        Task<bool> DeleteByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);
    }
}
