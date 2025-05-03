using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ResumenCtaCteProDetalle
    {
		public int? IdDocumento { get; set; }
		public DateTime? Fecha { get; set; }
        public int IdComprobante { get; set; }
        public string Comprobante { get; set; }
        public string Sucursal { get; set; }
        public string Numero { get; set; }
        public bool EsCredito { get; set; }
        public double? Debito { get; set; }
        public double? Credito { get; set; }
        public double? Saldo { get; set; }
        public double? Retenciones { get; set; }
        public double Total { get; set; }
    }
}
