using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.SubdiarioIVAVentas
{
    public class SubdiarioIVAVentasViewModel
    {
        public int IdCliente { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public SelectList MaestroClientes { get; set; }
    }
}