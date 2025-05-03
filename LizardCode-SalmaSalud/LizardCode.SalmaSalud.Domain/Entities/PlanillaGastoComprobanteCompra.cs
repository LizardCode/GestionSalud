using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("PlanillaGastosComprobantesCompras")]

    public class PlanillaGastoComprobanteCompra
    {
        [ExplicitKey]
        public int IdPlanillaGastos { get; set; }
        [ExplicitKey]
        public int IdComprobanteCompra { get; set; }

    }
}
