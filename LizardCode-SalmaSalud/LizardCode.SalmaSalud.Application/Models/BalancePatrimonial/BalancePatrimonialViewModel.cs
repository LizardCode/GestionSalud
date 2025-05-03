using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.BalancePatrimonial
{
    public class BalancePatrimonialViewModel
    {
        public int IdEjercicio { get; set; }
        public string FechaHasta { get; set; }
        public SelectList MaestroEjercicios { get; set; }

    }
}