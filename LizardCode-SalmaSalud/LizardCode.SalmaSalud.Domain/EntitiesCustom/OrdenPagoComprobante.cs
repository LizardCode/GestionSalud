using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class OrdenPagoComprobante
	{
		public bool Seleccionar { get; set; }
		public int IdComprobanteCompra { get; set; }
		public DateTime Fecha { get; set; }
		public string TipoComprobante { get; set; }
		public string NumeroComprobante { get; set; }
		public double Total { get; set; }
		public double Saldo { get; set; }
		public double Importe { get; set; }
		public bool EsCredito { get; set; }
        public double Cotizacion { get; set; }
    }
}
