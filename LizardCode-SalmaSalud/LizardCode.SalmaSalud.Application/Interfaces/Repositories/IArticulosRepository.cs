using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IArticulosRepository
    {
        Task<IList<TArticulo>> GetAll<TArticulo>(IDbTransaction transaction = null);

        Task<TArticulo> GetById<TArticulo>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TArticulo>(TArticulo entity, IDbTransaction transaction = null);

        Task<bool> Update<TArticulo>(TArticulo entity, IDbTransaction transaction = null);

        Task<IList<Articulo>> GetAllByIdEmpresa(int idEmpresa);

        DataTablesCustomQuery GetAllCustomQuery();
    }
}