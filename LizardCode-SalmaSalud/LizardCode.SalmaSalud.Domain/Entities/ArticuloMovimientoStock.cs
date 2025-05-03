using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ArticulosMovimientosStock")]

    public class ArticuloMovimientoStock
    {
        [Key]
        public int IdArticuloMovimientoStock { get; set; }
        public int IdArticulo { get; set; }
        public int? IdComprobanteVenta { get; set; }
        public int? IdComprobanteCompra { get; set; }
        public double Canitdad { get; set; }

    }
}
