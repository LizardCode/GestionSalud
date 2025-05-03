using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IEvolucionesArchivosRepository
    {
        Task<IList<TEvolucionArchivo>> GetAll<TEvolucionArchivo>(IDbTransaction transaction = null);

        Task<TEvolucionArchivo> GetById<TEvolucionArchivo>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TEvolucionArchivo>(TEvolucionArchivo entity, IDbTransaction transaction = null);

        Task<bool> Update<TEvolucionArchivo>(TEvolucionArchivo entity, IDbTransaction transaction = null);

        Task<IList<EvolucionArchivo>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);

        Task<bool> DeleteByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);
    }
}