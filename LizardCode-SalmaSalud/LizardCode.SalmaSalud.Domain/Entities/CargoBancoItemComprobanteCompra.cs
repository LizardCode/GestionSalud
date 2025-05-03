using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CargosBancoItemsComprobantesCompras")]

    public class CargoBancoItemComprobanteCompra
	{
        [ExplicitKey]
        public int IdCargoBanco { get; set; }

        [ExplicitKey]
        public int Item { get; set; }

        [ExplicitKey]
        public int IdComprobanteCompra { get; set; }

    }
}
