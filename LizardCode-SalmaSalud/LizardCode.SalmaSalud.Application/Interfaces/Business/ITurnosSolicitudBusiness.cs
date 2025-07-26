using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.PortalPacientes;
using LizardCode.SalmaSalud.Application.Models.TurnosSolicitud;
using LizardCode.SalmaSalud.Domain.Entities;
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
        Task<List<Custom.TurnoSolicitud>> GetTurnosPaciente();
        Task<int> New(TurnoSolicitudViewModel model);
        Task Remove(int idTurnoSolicitud);
        Task<int> Solicitar(NuevaSolicitudViewModel model);
        Task Update(TurnoSolicitudViewModel model);
        Task Cancelar(CancelarViewModel model, Paciente paciente);
        Task Asignar(AsignarViewModel model);
        Task<Custom.TurnoSolicitudTotales> ObtenerTotalesDashboard();
        Task<Custom.TurnoSolicitud> GetCustomById(int idSolicitud);
        Task Nuevo(NuevoViewModel model);
        Task ReAsignar(ReAsignarViewModel model);
        Task<DataTablesResponse<Custom.TurnosSolicitudDashboard>> TurnosSolicitudDashboard(DataTablesRequest request, int idEspecialidad);
    }
}