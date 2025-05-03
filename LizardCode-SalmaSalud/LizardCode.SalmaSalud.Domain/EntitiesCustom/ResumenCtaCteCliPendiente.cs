using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ResumenCtaCteCliPendiente
    {
		public int? IdComprobanteVenta { get; set; }
		public int? IdTipoComprobante { get; set; }
		public DateTime? Fecha { get; set; }
        public DateTime? FechaVto { get; set; }
		public int? IdEjercicio { get; set; }
		public int IdComprobante { get; set; }
        public string Comprobante { get; set; }
        public bool EsCredito { get; set; }
        public string Sucursal { get; set; }
        public string Numero { get; set; }
        public double Saldo { get; set; }
		public double SaldoAcumulado { get; set; }
	}
}
