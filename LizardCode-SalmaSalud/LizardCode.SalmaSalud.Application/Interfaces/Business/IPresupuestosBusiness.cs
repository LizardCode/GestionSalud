using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Presupuestos;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IPresupuestosBusiness
    {
        Task<PresupuestoViewModel> Get(int idPresupuesto);
        Task<DataTablesResponse<Custom.Presupuesto>> GetAll(DataTablesRequest request);
        Task New(PresupuestoViewModel model);
        Task Remove(int idPresupuesto);
        Task Update(PresupuestoViewModel model);
        Task<IList<PresupuestoPrestacion>> GetAllPrestacionesByPresupuestoId(int idPresupuesto);
        Task<PresupuestoPrestacion> GetPresupuestoPrestacionById(int idPresupuestoPrestacion);
        Task<IList<PresupuestoOtraPrestacion>> GetAllOtrasPrestacionesByPresupuestoId(int idPresupuesto);
        Task<PresupuestoOtraPrestacion> GetPresupuestoOtraPrestacionById(int idPresupuestoOtraPrestacion);
        Task Aprobar(int idPresupuesto);
        Task Rechazar(int idPresupuesto);
        Task<List<Custom.Presupuesto>> GetPresupuestosAprobados(int idPaciente);
        Task<IList<PresupuestoOtraPrestacionViewModel>> GetAllOtrasPrestacionesByPresupuestoIds(List<int> ids);
        Task<IList<PresupuestoPrestacionViewModel>> GetAllPrestacionesByPresupuestoIds(List<int> ids);
        Task<List<Custom.Presupuesto>> GetPresupuestosAprobadosDisponibles();
        Task<IList<Custom.PedidoLaboratorio>> GetPedidosPorPresupuesto(int idPresupuesto);
        Task Cerrar(int idPresupuesto);
    }
}