using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("RecibosAsientos")]
	
	public class ReciboAsiento
	{
		[ExplicitKey]
		public int IdRecibo { get; set; }
		[ExplicitKey]
		public int IdAsiento { get; set; }

	}
}
