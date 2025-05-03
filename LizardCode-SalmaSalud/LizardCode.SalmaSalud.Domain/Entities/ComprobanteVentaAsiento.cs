using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ComprobantesVentasAsientos")]

    public class ComprobanteVentaAsiento
    {
        [ExplicitKey]
        public int IdComprobanteVenta { get; set; }
        [ExplicitKey]
        public int IdAsiento { get; set; }

    }
}
