using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.SalmaSalud.Application.Models.Evoluciones;

namespace LizardCode.SalmaSalud.Application.Models.Turnos
{
    public class TurnoGuardiaViewModel
    {
        public EvolucionRecetaViewModel Receta { get; set; }
        public EvolucionOrdenViewModel Orden { get; set; }
        public SelectList MaestroVademecum { get; set; }
    }
}
