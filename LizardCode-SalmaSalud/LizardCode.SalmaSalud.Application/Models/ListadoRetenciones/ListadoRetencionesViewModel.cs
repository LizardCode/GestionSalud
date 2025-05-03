using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.ListadoRetenciones
{
    public class ListadoRetencionesViewModel
    {
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public int IdTipoRetecion { get; set; }
        public SelectList MaestroTipoRetecion { get; set; }

    }
}