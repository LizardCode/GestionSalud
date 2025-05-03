using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("PlanillaGastosItems")]

    public class PlanillaGastoItem
	{
        [ExplicitKey]
		public int IdPlanillaGastos { get; set; }
		[ExplicitKey]
		public int Item { get; set; }
		public string CUIT { get; set; }
		public string Proveedor { get; set; }
		public DateTime Fecha { get; set; }
		public int IdComprobante { get; set; }
		public string NumeroComprobante { get; set; }
        public string Detalle { get; set; }
        public double? SubtotalNoGravado { get; set; }
        public double Subtotal { get; set; }
        public int IdAlicuota { get; set; }
        public double Alicuota { get; set; }
        public double? Subtotal2 { get; set; }
        public int? IdAlicuota2 { get; set; }
        public double? Alicuota2 { get; set; }
		public int? IdCuentaContablePercepcion { get; set; }
		public double? Percepcion { get; set; }
		public int? IdCuentaContablePercepcion2 { get; set; }
		public double? Percepcion2 { get; set; }
        public double? ImpuestosInternos { get; set; }
        public double Total { get; set; }
        public string CAE { get; set; }
        public DateTime? VencimientoCAE { get; set; }
    }
}
