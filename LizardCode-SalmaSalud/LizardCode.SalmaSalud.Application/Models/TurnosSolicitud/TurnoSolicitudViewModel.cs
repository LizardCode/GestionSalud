using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.TurnosSolicitud
{
    public class TurnoSolicitudViewModel
    {
        public TurnoSolicitudViewModel() 
        {
            FechaDesde = DateTime.Now.AddDays(-30).Date;
            FechaHasta = DateTime.Now.AddDays(30).Date;
        }

        public int IdTurnoSolicitud{ get; set; }

        [RequiredEx]
        public int IdEspecialidad { get; set; }

        [RequiredEx]
        public int IdPaciente { get; set; }

        public int Dia { get; set; }
        public int IdRangoHorario { get; set; }

        public string Observaciones { get; set; }

        public SelectList MaestroDias { get; set; }
        public SelectList MaestroRangosHorarios { get; set; }

        public SelectList MaestroEspecialidades { get; set; }
        public SelectList MaestroPacientes { get; set; }

        #region Filtros

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public int IdEstadoTurnoSolicitud { get; set; }

        #endregion
    }
}
