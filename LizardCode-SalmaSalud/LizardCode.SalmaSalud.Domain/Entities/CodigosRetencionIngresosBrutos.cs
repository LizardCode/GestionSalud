using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CodigosRetencionIngresosBrutos")]
    public class CodigosRetencionIngresosBrutos
    {
        [ExplicitKey]
        public int IdCodigoRetencion { get; set; }
        public double ImporteNoSujeto { get; set; }
        public double PorcentajeRetencion { get; set; }
        public bool PadronRetencionAGIP { get; set; }
        public bool PadronRetencionARBA { get; set; }
    }
}
