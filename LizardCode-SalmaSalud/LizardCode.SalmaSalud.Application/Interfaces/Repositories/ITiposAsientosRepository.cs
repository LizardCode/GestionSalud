using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ITiposAsientosRepository
    {
        Task<IList<TTiposAsientos>> GetAll<TTiposAsientos>(IDbTransaction transaction = null);

        Task<TTiposAsientos> GetById<TTiposAsientos>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TTiposAsientos>(TTiposAsientos entity, IDbTransaction transaction = null);

        Task<bool> Update<TTiposAsientos>(TTiposAsientos entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
        Task<IList<TipoAsiento>> GetTiposAsientoByIdEmpresa(int idEmpresa);
        Task<List<TipoAsientoCuenta>> GetItemsByIdTipoAsiento(int idTipoAsiento, IDbTransaction transaction = null);
    }
}