using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ComprobantesVentasItems")]

    public class ComprobanteVentaItem
    {
		[ExplicitKey]
		public int IdComprobanteVenta { get; set; }
		[ExplicitKey]
		public int Item { get; set; }
		public int? IdArticulo { get; set; }
        public int? IdEvolucionPrestacion { get; set; }
        public int? IdEvolucionOtraPrestacion { get; set; }
        public string Descripcion { get; set; }
		public double Cantidad { get; set; }
		public double Precio { get; set; }
		public double Bonificacion { get; set; }
		public double Importe { get; set; }
		public double Impuestos { get; set; }
		public double Alicuota { get; set; }

	}
}
