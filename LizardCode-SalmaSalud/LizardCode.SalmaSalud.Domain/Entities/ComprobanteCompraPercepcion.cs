using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ComprobantesComprasPercepciones")]

    public class ComprobanteCompraPercepcion
	{
		[ExplicitKey]
		public int IdComprobanteCompra { get; set; }
		[ExplicitKey]
		public int IdCuentaContable { get; set; }
		public double Importe { get; set; }

	}
}
