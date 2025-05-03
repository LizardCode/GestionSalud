using LizardCode.Framework.Application.Common.Annotations;

namespace LizardCode.SalmaSalud.Application.Models.Turnos
{
    public class CancelarViewModel
    {
        public int IdTurno { get; set; }
        [RequiredEx]
        public string Observaciones { get; set; }
    }
}
