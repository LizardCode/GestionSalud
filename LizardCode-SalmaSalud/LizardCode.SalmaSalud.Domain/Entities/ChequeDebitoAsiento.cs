using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ChequesDebitosAsientos")]

    public class ChequeDebitoAsiento
    {
        [ExplicitKey]
        public int IdCheque { get; set; }
        [ExplicitKey]
        public int IdAsiento { get; set; }

    }
}
