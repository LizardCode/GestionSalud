using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("OrdenesPagoAsientos")]
	
	public class OrdenPagoAsiento
	{
		[ExplicitKey]
		public int IdOrdenPago { get; set; }
		[ExplicitKey]
		public int IdAsiento { get; set; }

	}
}
