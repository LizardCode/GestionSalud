using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.SubdiarioIVACompras
{
    public class SubdiarioIVAComprasViewModel
    {
        public int IdProveedor { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public SelectList MaestroProveedores { get; set; }

    }
}