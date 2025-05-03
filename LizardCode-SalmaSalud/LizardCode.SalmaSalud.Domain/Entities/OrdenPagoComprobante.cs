using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("OrdenesPagoComprobantes")]
	
	public class OrdenPagoComprobante
    {
		[Key]
		public int IdOrdenPagoComprobante { get; set; }
		public int IdOrdenPago { get; set; }
		public int IdComprobanteCompra { get; set; }
		public double Importe { get; set; }
        public double Cotizacion { get; set; }
    }
}
