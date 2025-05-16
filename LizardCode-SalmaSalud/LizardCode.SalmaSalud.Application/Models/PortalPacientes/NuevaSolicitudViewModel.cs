using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.PortalPacientes
{
    public class NuevaSolicitudViewModel
    {
        public int IdPaciente { get; set; }

        [RequiredEx]
        public int IdEspecialidad { get; set; }

        public List<int> Dias { get; set; }
        public List<int> RangosHorarios { get; set; }

        public string Observaciones { get; set; }

        public SelectList MaestroDias { get; set; }
        public SelectList MaestroRangosHorarios { get; set; }

        public SelectList MaestroEspecialidades { get; set; }
    }
}