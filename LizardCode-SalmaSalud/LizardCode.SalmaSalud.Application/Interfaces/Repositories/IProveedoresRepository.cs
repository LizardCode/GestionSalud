using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IProveedoresRepository
    {
        Task<IList<TProveedor>> GetAll<TProveedor>(IDbTransaction transaction = null);

        Task<TProveedor> GetById<TProveedor>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TProveedor>(TProveedor entity, IDbTransaction transaction = null);

        Task<bool> Update<TProveedor>(TProveedor entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<bool> ValidarCUITExistente(string cuit, int? id, int idEmpresa, IDbTransaction transaction = null);

        Task<List<Proveedor>> GetAllProveedoresByIdEmpresaLookup(int idEmpresa);

        Task<List<CodigosRetencion>> GetCodigosRetencionByIdProveedor(int idProveedor, IDbTransaction transaction = null);

        Task<Proveedor> GetByCUIT(string cuit, int idEmpresa, IDbTransaction transaction = null);

        Task<Proveedor> GetProveedorByCUIT(string cuit, IDbTransaction transaction = null);
        Task<Proveedor> GetProveedorByIdProfesional(int idProfesional, IDbTransaction transaction = null);
    }
}