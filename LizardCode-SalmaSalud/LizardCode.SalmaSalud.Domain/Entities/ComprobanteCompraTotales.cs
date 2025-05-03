using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ComprobantesComprasTotales")]

    public class ComprobanteCompraTotales
	{
		[ExplicitKey]
		public int IdComprobanteCompra { get; set; }
		[ExplicitKey]
		public int Item { get; set; }
		public double Neto { get; set; }
		public double ImporteAlicuota { get; set; }
		public double Alicuota { get; set; }
	}
}
