using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ISucursalesNumeracionRepository
    {
        Task<IList<Domain.EntitiesCustom.SucursalNumeracion>> GetAllCustomSucursalesNumByIdSucursal(int idSucursal, int idEmpresa);

        Task<bool> Insert(SucursalNumeracion entity, IDbTransaction transaction = null);

        Task<bool> Update(SucursalNumeracion entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdSucursal(int idSucursal, IDbTransaction transaction = null);

        Task<bool> DeleteByIdSucursalAndComprobante(int idSucursal, int idComprobante, IDbTransaction transaction = null);

        Task<SucursalNumeracion> GetByIdSucursalAndComprobante(int idSucursal, int idComprobante, IDbTransaction transaction = null);
        
        Task<SucursalNumeracion> GetLastNumeroByComprobante(int idComprobante, int idSucursal, int idEmpresa, IDbTransaction transaction = null);
    }
}