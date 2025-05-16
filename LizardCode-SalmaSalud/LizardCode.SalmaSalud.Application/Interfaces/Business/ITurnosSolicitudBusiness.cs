using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.PortalPacientes;
using LizardCode.SalmaSalud.Application.Models.TurnosSolicitud;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ITurnosSolicitudBusiness
    {
        Task<TurnoSolicitudViewModel> Get(int idTurnoSolicitud);
        Task<DataTablesResponse<Custom.TurnoSolicitud>> GetAll(DataTablesRequest request);
        Task<List<TurnoSolicitud>> GetTurnosPaciente();
        Task New(TurnoSolicitudViewModel model);
        Task Remove(int idTurnoSolicitud);
        Task Solicitar(NuevaSolicitudViewModel model);
        Task Update(TurnoSolicitudViewModel model);
        Task Cancelar(CancelarViewModel model);
        Task Asignar(AsignarViewModel model);
        Task<TurnoSolicitudTotales> ObtenerTotalesDashboard();
    }
}