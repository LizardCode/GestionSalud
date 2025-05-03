using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("SaldoCtaCteCliComprobantesVentas")]
    public class SdoCtaCteCliComprobantesVentas
    {
        [Key]
        public int IdSaldoCtaCteCliComprobantesVentas { get; set; }

        public long IdSaldoCtaCteCli { get; set; }

        public long IdComprobanteVenta { get; set; }

        public int Item { get; set; }
    }
}
