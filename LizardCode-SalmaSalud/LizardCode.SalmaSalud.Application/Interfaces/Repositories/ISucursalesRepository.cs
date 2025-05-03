using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ISucursalesRepository
    {
        Task<IList<TSucursal>> GetAll<TSucursal>(IDbTransaction transaction = null);

        Task<TSucursal> GetById<TSucursal>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TSucursal>(TSucursal entity, IDbTransaction transaction = null);

        Task<bool> Update<TSucursal>(TSucursal entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<IList<Sucursal>> GetAllSucursalesByIdEmpresa(int idEmpresa);

        Task<string> GetAFIPConsultaNumeracion(AfipAuth afipAuth, string cuit, int codigoSucursal, int codigo, bool useProd);
    }
}