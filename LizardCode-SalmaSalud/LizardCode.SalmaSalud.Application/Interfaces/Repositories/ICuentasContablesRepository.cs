using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICuentasContablesRepository
    {
        Task<IList<TCuentaContable>> GetAll<TCuentaContable>(IDbTransaction transaction = null);

        Task<TCuentaContable> GetById<TCuentaContable>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCuentaContable>(TCuentaContable entity, IDbTransaction transaction = null);

        Task<bool> Update<TCuentaContable>(TCuentaContable entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<IList<CuentaContable>> GetCuentasContablesByIdEmpresa(int idEmpresa, IDbTransaction transaction = null);

        Task<CuentaContable> GetCuentaContablesByIdEmpresaAndCodObservacion(int idEmpresa, int idCodigoObservacion, IDbTransaction transaction = null);

        Task<CuentaContable> GetByIdBanco(int idBanco, int idEmpresa, IDbTransaction transaction = null);

        Task<List<Domain.EntitiesCustom.MayorCuentas>> GetMayorCuentas(Dictionary<string, object> filters);

        Task<CuentaContable> GetCuentaContablesByIdEmpresaAndDescripcion(int idEmpresa, string descripcionCuentaContable, IDbTransaction transaction = null);
    }
}