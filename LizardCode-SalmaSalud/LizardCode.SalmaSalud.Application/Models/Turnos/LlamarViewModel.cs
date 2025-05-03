using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.Turnos
{
    public class LlamarViewModel
    {
        public int IdTurno { get; set; }
        public int IdPaciente { get; set; }

        [RequiredEx]
        public int IdConsultorio { get; set; }
        //public string Consultorio { get; set; }

        //[RequiredEx]
        public string Observaciones { get; set; }
        public SelectList MaestroConsultorios { get; set; }
    }
}
