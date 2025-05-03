using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.Turnos
{
    public class RecepcionarViewModel
    {
        public int IdTurno { get; set; }
        public int IdPaciente { get; set; }

        public bool ForzarParticular { get; set; }
        public bool ForzarPadron { get; set; }
        public string FinanciadorNro { get; set; }

        [RequiredEx]
        public int IdConsultorio { get; set; }
        //public string Consultorio { get; set; }

        //[RequiredEx]
        public string Observaciones { get; set; }
        public SelectList MaestroConsultorios { get; set; }
    }
}
