using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Turnos
{
    public class CancelarAgendaViewModel
    {
        [RequiredEx]
        public DateTime FechaCancelacion { get; set; }

        [RequiredEx]
        public int IdProfesional { get; set; }

        [RequiredEx]
        public string Motivo { get; set; }

        public SelectList MaestroProfesionales { get; set; }
    }
}
