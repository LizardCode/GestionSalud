using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Sucursales;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ISucursalesBusiness
    {
        Task Update(SucursalesViewModel model);
        Task Remove(int idSucursal);
        Task New(SucursalesViewModel model);
        Task<SucursalesViewModel> Get(int idSucursal);
        Task<DataTablesResponse<Sucursal>> GetAll(DataTablesRequest request);
        Task<IList<Domain.EntitiesCustom.SucursalNumeracion>> GetSucursalesNumeracionByIdSucursal(int idSucursal);
        Task ActualizaNumeracion(int idSucursal, int idComprobante, string numerador);
        Task<string> AFIPConsultaNumeracion(int idSucursal, int idComprobante);
    }
}