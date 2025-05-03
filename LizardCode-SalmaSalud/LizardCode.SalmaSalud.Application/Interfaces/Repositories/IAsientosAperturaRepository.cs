using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IAsientosAperturaRepository
    {
        Task<IList<TAsientoApertura>> GetAll<TAsientoApertura>(IDbTransaction transaction = null);

        Task<TAsientoApertura> GetById<TAsientoApertura>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TAsientoApertura>(TAsientoApertura entity, IDbTransaction transaction = null);

        Task<bool> Update<TAsientoApertura>(TAsientoApertura entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<Custom.AsientoAperturaCierre> GetByIdCustom(int idAsientoApertura, IDbTransaction transaction = null);

    }
}