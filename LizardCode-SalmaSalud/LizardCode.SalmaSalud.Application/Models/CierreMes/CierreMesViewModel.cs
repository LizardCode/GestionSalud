using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.CierreMes
{
    public class CierreMesViewModel
    {
        public int IdEjercicio { get; set; }

        public SelectList MaestroEjercicios { get; set; }

    }
}