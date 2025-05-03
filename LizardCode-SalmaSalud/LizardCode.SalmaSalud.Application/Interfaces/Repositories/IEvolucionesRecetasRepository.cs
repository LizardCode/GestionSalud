using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IEvolucionesRecetasRepository
    {
        Task<IList<TEvolucionReceta>> GetAll<TEvolucionReceta>(IDbTransaction transaction = null);

        Task<TEvolucionReceta> GetById<TEvolucionReceta>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TEvolucionReceta>(TEvolucionReceta entity, IDbTransaction transaction = null);

        Task<bool> Update<TEvolucionReceta>(TEvolucionReceta entity, IDbTransaction transaction = null);

        Task<IList<EvolucionReceta>> GetAllByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);

        Task<bool> DeleteByIdEvolucion(int idEvolucion, IDbTransaction transaction = null);
    }
}
