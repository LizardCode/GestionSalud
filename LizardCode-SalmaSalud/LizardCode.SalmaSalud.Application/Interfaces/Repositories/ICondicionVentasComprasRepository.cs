using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICondicionVentasComprasRepository
    {
        Task<IList<TCondicionVentaCompra>> GetAll<TCondicionVentaCompra>(IDbTransaction transaction = null);

        Task<TCondicionVentaCompra> GetById<TCondicionVentaCompra>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCondicionVentaCompra>(TCondicionVentaCompra entity, IDbTransaction transaction = null);

        Task<bool> Update<TCondicionVentaCompra>(TCondicionVentaCompra entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
    }
}