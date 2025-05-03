using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CargosBancoAsientos")]

    public class CargoBancoAsiento
	{
        [ExplicitKey]
		public int IdCargoBanco { get; set; }
        [ExplicitKey]
        public int IdAsiento { get; set; }

    }
}
