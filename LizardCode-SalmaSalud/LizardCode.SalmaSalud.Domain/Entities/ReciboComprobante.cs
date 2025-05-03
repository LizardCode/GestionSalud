using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("RecibosComprobantes")]
	
	public class ReciboComprobante
    {
		[Key]
		public int IdReciboComprobante { get; set; }
		public int IdRecibo { get; set; }
		public int? IdComprobanteVenta { get; set; }
		public double Importe { get; set; }
        public double Cotizacion { get; set; }

    }
}