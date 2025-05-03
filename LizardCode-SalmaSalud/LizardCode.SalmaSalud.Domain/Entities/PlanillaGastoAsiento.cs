using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("PlanillaGastosAsientos")]

    public class PlanillaGastoAsiento
	{
        [ExplicitKey]
		public int IdPlanillaGastos { get; set; }
		[ExplicitKey]
		public int IdAsiento { get; set; }

	}
}
