using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("DepositosBancoAsientos")]
	
	public class DepositoBancoAsiento
	{
		[ExplicitKey]
		public int IdDepositoBanco { get; set; }
		[ExplicitKey]
		public int IdAsiento { get; set; }
	}
}
