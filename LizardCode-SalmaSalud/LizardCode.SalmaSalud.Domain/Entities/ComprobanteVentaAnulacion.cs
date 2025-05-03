using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ComprobantesVentasAnulaciones")]

    public class ComprobanteVentaAnulacion
    {
		[ExplicitKey]
		public int IdComprobanteVenta { get; set; }
		[ExplicitKey]
		public int IdComprobanteVentaAnulado { get; set; }

	}
}
