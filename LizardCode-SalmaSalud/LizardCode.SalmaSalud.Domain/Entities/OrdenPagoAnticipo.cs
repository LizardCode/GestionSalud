using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("OrdenesPagoAnticipos")]
	
	public class OrdenPagoAnticipo
    {
		[Key]
		public int IdOrdenPagoAnticipo { get; set; }
		public int IdOrdenPago { get; set; }
		public int? IdAnticipo { get; set; }
		public double Importe { get; set; }
	}
}
