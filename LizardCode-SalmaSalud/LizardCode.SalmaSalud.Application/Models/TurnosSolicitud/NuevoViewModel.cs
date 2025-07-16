using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.TurnosSolicitud
{
    public class NuevoViewModel
    {
        [RequiredEx]
        public DateTime? Fecha { get; set; }

        public string Observaciones { get; set; }

        public string Dias { get; set; }
        public string Rangos { get; set; }

        [RequiredEx]
        public int IdProfesional { get; set; }
        public SelectList MaestroProfesionales { get; set; }

        [RequiredEx]
        public int IdPaciente { get; set; }
        public SelectList MaestroPacientes { get; set; }

        [RequiredEx]
        public int IdEspecialidad { get; set; }
        public SelectList MaestroEspecialidades { get; set; }
    }
}
