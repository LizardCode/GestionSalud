using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ComprobantesComprasItems")]

    public class ComprobanteCompraItem
	{
		[ExplicitKey]
		public int IdComprobanteCompra { get; set; }
		[ExplicitKey]
		public int Item { get; set; }
		public int? IdArticulo { get; set; }
        public int? IdCuentaContable { get; set; }
        public string Descripcion { get; set; }
		public double Cantidad { get; set; }
		public double Precio { get; set; }
		public double Bonificacion { get; set; }
		public double Importe { get; set; }
		public double Impuestos { get; set; }
		public double Alicuota { get; set; }

	}
}
