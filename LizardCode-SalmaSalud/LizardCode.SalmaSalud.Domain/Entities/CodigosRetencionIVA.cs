using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CodigosRetencionIVA")]
    public class CodigosRetencionIVA
    {
        [ExplicitKey]
        public int IdCodigoRetencion { get; set; }
        public double ImporteNoSujeto { get; set; }
        public double PorcentajeRetencion { get; set; }
    }
}
