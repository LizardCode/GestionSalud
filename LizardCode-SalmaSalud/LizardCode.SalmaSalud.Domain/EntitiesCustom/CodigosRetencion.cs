using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class CodigosRetencion : Entities.CodigosRetencion
    {
        public string TipoRetencion { get; set; }

        public double ImporteNoSujeto { get; set; }

        public double? ImporteMinimoRetencion { get; set; }

        public double? PorcentajeRetencion { get; set; }

        public int? CantidadMeses { get; set; }

        public bool PadronRetencionAGIP { get; set; }

        public bool PadronRetencionARBA { get; set; }

        public bool AcumulaPagos { get; set; }

        public List<Entities.CodigosRetencionGananciasItems> Items { get; set; }

    }
}
