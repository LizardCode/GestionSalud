using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.MayorCuentas
{
    public class MayorCuentasViewModel
    {
        public int IdEjercicio { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public int IdCuentaDesde { get; set; }
        public int IdCuentaHasta { get; set; }

        public SelectList MaestroCuentas { get; set; }
        public SelectList MaestroEjercicios { get; set; }

    }
}