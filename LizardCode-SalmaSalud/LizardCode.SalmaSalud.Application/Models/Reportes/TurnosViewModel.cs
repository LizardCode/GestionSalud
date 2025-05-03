using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Reportes
{
    public class TurnosViewModel
    {
        public int IdTipoTurno { get; set; }
        public int IdEstadoTurno { get; set; }
        public int IdProfesional { get; set; }
        public int IdFinanciador { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }

        public SelectList MaestroTiposTurno { get; set; }
        public SelectList MaestroEstadosTurno { get; set; }
        public SelectList MaestroProfesionales { get; set; }
        //public SelectList MaestroEspecialidades { get; set; }
        public SelectList MaestroFinanciadores { get; set; }
    }
}