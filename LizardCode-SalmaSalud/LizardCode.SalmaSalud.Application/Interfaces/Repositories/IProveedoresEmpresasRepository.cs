using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IProveedoresEmpresasRepository
    {
        Task<IList<TProveedorEmpresa>> GetAll<TProveedorEmpresa>(IDbTransaction transaction = null);

        Task<TProveedorEmpresa> GetById<TProveedorEmpresa>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TProveedorEmpresa>(TProveedorEmpresa entity, IDbTransaction transaction = null);

        Task<bool> Update<TProveedorEmpresa>(TProveedorEmpresa entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdProveedor(int idProveedor, int idUsuario, IDbTransaction transaction = null);

        Task<IList<ProveedorEmpresa>> GetAllByIdProveedor(int idProveedor, IDbTransaction transaction = null);

        Task<ProveedorEmpresa> GetByIdProveedorAndEmpresa(int idProveedor, int idEmpresa, IDbTransaction transaction = null);
    }
}