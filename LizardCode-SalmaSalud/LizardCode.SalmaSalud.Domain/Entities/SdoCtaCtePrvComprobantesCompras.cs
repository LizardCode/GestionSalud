using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("SaldoCtaCtePrvComprobantesCompras")]
    public class SdoCtaCtePrvComprobantesCompras
    {
        [Key]
        public int IdSaldoCtaCtePrvComprobantesCompras { get; set; }

        public long IdSaldoCtaCtePrv { get; set; }

        public long IdComprobanteCompra { get; set; }

        public int Item { get; set; }
    }
}
