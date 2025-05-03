using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ComprobantesVentasTotales")]

    public class ComprobanteVentaTotales
	{
		[ExplicitKey]
		public int IdComprobanteVenta { get; set; }
		[ExplicitKey]
		public int Item { get; set; }
		public double Neto { get; set; }
		public double ImporteAlicuota { get; set; }
		public double Alicuota { get; set; }
		public int IdTipoAlicuota { get; set; }
	}
}
