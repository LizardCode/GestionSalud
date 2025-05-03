using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.ResumenCtaCteCli
{
    public class ResumenCtaCteCliViewModel
    {
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
		public int? SaldosEnCero { get; set; }
		public int IdCliente { get; set; }
        public SelectList MaestroClientes { get; set; }
		public SelectList MaestroSaldosEnCero { get; set; }

	}
}