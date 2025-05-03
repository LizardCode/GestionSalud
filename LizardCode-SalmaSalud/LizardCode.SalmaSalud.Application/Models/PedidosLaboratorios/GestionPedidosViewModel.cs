using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.PedidosLaboratorios
{
    public class GestionPedidosViewModel
    {
        public int IdPedidoLaboratorio { get; set; }
        public int IdLaboratorio { get; set; }
        public int IdEstadoPedidoLaboratorio { get; set; }
        public int IdPresupuesto { get; set; }

        public DateTime Fecha { get; set; }
        public DateTime? FechaEnviado { get; set; }
        public DateTime? FechaRecibido { get; set; }

        public string Estado { get; set; }
        public string EstadoClase { get; set; }
        public string Laboratorio { get; set; }
        public string Paciente { get; set; }
        public string PacienteDocumento { get; set; }

        #region Filters

        public DateTime FiltroFechaDesde { get; set; }
        public DateTime FiltroFechaHasta { get; set; }

        public int IdPacienteFilter { get; set; }
        public int IdLaboratorioFilter { get; set; }

        public SelectList MaestroPacientes { get; set; }
        public SelectList MaestroLaboratorios { get; set; }
        public SelectList MaestroEstados { get; set; }

        #endregion
    }

    public class GestionViewModel
    {
        public int IdPedidoLaboratorio { get; set; }
        public string Pedido { get; set; }

        public List<GestionItemViewModel> Items { get; set; } = new List<GestionItemViewModel>();
    }

    public class GestionItemViewModel
    {
        public int Id { get; set; }
        public string Optica { get; set; }
        public string Item { get; set; }
        public int IdEstado { get; set; }
        public string Estado { get; set; }
        public string EstadoClase { get; set; }

        public DateTime FechaAdjudicacion { get; set; }
        public DateTime? FechaEnviadoLaboratorio { get; set; }
        public DateTime? FechaRecepcionLaboratorio { get; set; }
        public DateTime? FechaFinalizado { get; set; }
        public DateTime? FechaEnviadoOptica { get; set; }
        public DateTime? FechaRecepcionOptica { get; set; }
    }

    public class EnviarItemViewModel
    {
        public string Label { get; set; }
        public string Accion { get; set; }

        [RequiredEx]
        public string IdsPedidos { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; } = new DateTime();
        public string Observaciones { get; set; }

        [RequiredEx]
        public DateTime FechaEstimada { get; set; } = new DateTime();

        public int? NumeroSobre { get; set; }
    }

    public class MarcarItemViewModel
    {
        public string Label { get; set; }
        public string Accion { get; set; }

        [RequiredEx]
        public string IdsPedidos { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; } = new DateTime();
        public string Observaciones { get; set; }
    }

    public class PedidoLaboratorioHistorialViewModel
    {
        public int IdPedidoLaboratorio { get; set; }
        public string Item { get; set; }
    }
}
