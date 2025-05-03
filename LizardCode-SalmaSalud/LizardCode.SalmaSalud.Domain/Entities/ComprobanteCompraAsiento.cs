using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ComprobantesComprasAsientos")]

    public class ComprobanteCompraAsiento
    {
        [ExplicitKey]
        public int IdComprobanteCompra { get; set; }
        [ExplicitKey]
        public int IdAsiento { get; set; }

    }
}
