using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Laboratorios;
using LizardCode.SalmaSalud.Application.Models.Proveedores;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IProveedoresBusiness
    {
        Task<ProveedorViewModel> Get(int idProveedor);
        Task<DataTablesResponse<Custom.Proveedor>> GetAll(DataTablesRequest request, bool esLaboratorio);
        Task New(ProveedorViewModel model);
        Task Remove(int idProveedor);
        Task Update(ProveedorViewModel model);
        Task<string> ValidarNroCUIT(string cuit, int? idProveedor);
        Task<Domain.EntitiesCustom.Contribuyente> GetPadron(string cuit);

        Task<LaboratorioViewModel> GetLaboratorio(int idLaboratorio);
        Task NewLaboratorio(LaboratorioViewModel model);
        Task RemoveLaboratorio(int idLaboratorio);
        Task UpdateLaboratorio(LaboratorioViewModel model);
        Task<IList<LaboratorioServicio>> GetServiciosByIdLaboratorio(long idLaboratorio);
        Task<LaboratorioServicio> GetServicioById(int idLaboratorioServicio);
    }
}