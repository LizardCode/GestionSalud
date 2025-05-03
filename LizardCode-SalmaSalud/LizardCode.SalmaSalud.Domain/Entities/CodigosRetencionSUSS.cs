using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CodigosRetencionSUSS")]
    public class CodigosRetencionSUSS
    {
        [ExplicitKey]
        public int IdCodigoRetencion { get; set; }
        public double ImporteNoSujeto { get; set; }
        public double PorcentajeRetencion { get; set; }
    }
}
