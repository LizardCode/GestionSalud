using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CodigosRetencionGananciasItems")]
    public class CodigosRetencionGananciasItems
    {
        [ExplicitKey]
        public int IdCodigoRetencion { get; set; }
        [ExplicitKey]
        public int Item { get; set; }
        public double ImporteDesde { get; set; }
        public double ImporteHasta { get; set; }
        public double ImporteRetencion { get; set; }
        public double MasPorcentaje { get; set; }
        public double SobreExcedente { get; set; }
    }
}
