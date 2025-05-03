using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.SalmaSalud.Application.Models.Presupuestos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.PedidosLaboratorios
{
    public class PedidoLaboratorioViewModel
    {
        public int IdPedidoLaboratorio { get; set; }

        [RequiredEx]
        public int IdPresupuesto { get; set; }

        [RequiredEx]
        public int IdLaboratorio { get; set; }

        //[RequiredEx]
        public string Observaciones { get; set; }

        public List<PedidoLaboratorioServicioViewModel> Servicios { get; set; }

        #region Filters

        public DateTime FiltroFechaDesde { get; set; }
        public DateTime FiltroFechaHasta { get; set; }

        public int IdPacienteFilter { get; set; }
        public int IdLaboratorioFilter { get; set; }
        public int IdEstadoPedidoLaboratorio { get; set; }

        public SelectList MaestroPacientes { get; set; }
        public SelectList MaestroLaboratorios { get; set; }
        public SelectList MaestroEstados { get; set; }

        #endregion

        public SelectList MaestroServicios { get; set; }
        public SelectList MaestroPresupuestos { get; set; }
    }
}