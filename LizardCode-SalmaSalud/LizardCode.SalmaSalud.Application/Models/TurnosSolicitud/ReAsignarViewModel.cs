using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.TurnosSolicitud
{
    public class ReAsignarViewModel
    {
        public int IdTurnoSolicitud { get; set; }

        [RequiredEx]
        public DateTime? Fecha { get; set; }

        public string Observaciones { get; set; }

        public string Dias { get; set; }
        public string Rangos { get; set; }

        [RequiredEx]
        public int IdProfesional { get; set; }
        public string Profesional { get; set; }
        public SelectList MaestroProfesionales { get; set; }
    }
}
