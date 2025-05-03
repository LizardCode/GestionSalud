using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CodigosRetencionGanancias")]
    public class CodigosRetencionGanancias
    {
        [ExplicitKey]
        public int IdCodigoRetencion { get; set; }
        public double ImporteNoSujeto { get; set; }
        public double ImporteMinimoRetencion { get; set; }
        public bool AcumulaPagos { get; set; }

    }
}
