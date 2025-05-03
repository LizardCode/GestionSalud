using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.FullCalendar;
using LizardCode.SalmaSalud.Application.Models.Evoluciones;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Application.Models.Evoluciones.Odontograma;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Application.Models.Reportes;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IEvolucionesBusiness
    {
        Task<EvolucionViewModel> Get(int idEvolucion);
        Task<DataTablesResponse<Custom.Evolucion>> GetAll(DataTablesRequest request, int idPaciente = 0);
        Task<Custom.Evolucion> GetCustomById(int idEvolucion);
        Task<List<Custom.Evolucion>> GetEvolucionesPaciente();
		Task<OdontogramaViewModel> GetOdontograma(int idEvolucion);
        Task<List<Custom.PrestacionProfesional>> GetPrestacionesProfesional(DataTablesRequest request);
        Task<List<Custom.PrestacionFinanciador>> GetPrestacionesFinanciador(DataTablesRequest request);
        Task<List<ResumenImagenesViewModel>> GetResumenImagenes(int idEvolucion);
        Task<List<ResumenOrdenesViewModel>> GetResumenOrdenes(int idEvolucion);
        Task<List<ResumenPrestacionesViewModel>> GetResumenPrestaciones(int idEvolucion);
        Task<List<ResumenRecetasViewModel>> GetResumenRecetas(int idEvolucion);
        Task<OdontogramaViewModel> GetUltimoOdontograma(int idPaciente);
		Task New(EvolucionViewModel model);
        Task Remove(int idEvolucion);
        Task Update(EvolucionViewModel model);
        Task<EvolucionesEstadisticas> GetEstadisticas(EvolucionesEstadisticasViewModel filters);
        Task<List<EvolucionesEstadisticasFinanciador>> GetEstadisticasFinanciador(EvolucionesEstadisticasViewModel filters);
        Task<List<EvolucionesEstadisticasEspecialidad>> GetEstadisticasEspecialidad(EvolucionesEstadisticasViewModel filters);
        Task NewExterna(EvolucionExternaViewModel model, IDbTransaction transaction);
        Task<EvolucionesImportarExcelResultViewModel> ImportarEvolucionesExcel(IFormFile file);
    }
}
