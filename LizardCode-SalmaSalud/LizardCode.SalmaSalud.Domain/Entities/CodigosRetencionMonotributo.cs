using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CodigosRetencionMonotributo")]
    public class CodigosRetencionMonotributo
    {
        [ExplicitKey]
        public int IdCodigoRetencion { get; set; }
        public double ImporteNoSujeto { get; set; }
        public double PorcentajeRetencion { get; set; }
        public int CantidadMeses { get; set; }
    }
}
