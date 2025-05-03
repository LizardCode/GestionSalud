using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("OrdenesPagoRetenciones")]
	
	public class OrdenPagoRetencion
    {
		[Key]
		public int IdOrdenPagoRetencion { get; set; }
		public int IdOrdenPago { get; set; }
		public int IdTipoRetencion { get; set; }
		public DateTime Fecha { get; set; }
		public double BaseImponible { get; set; }
		public double Importe { get; set; }
	}
}
