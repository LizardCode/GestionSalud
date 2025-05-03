using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.ResumenCtaCtePro
{
    public class ResumenCtaCteProViewModel
    {
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public int IdProveedor { get; set; }
        public SelectList MaestroProveedores { get; set; }
		public int? SaldosEnCero { get; set; }
		public SelectList MaestroSaldosEnCero { get; set; }

	}
}