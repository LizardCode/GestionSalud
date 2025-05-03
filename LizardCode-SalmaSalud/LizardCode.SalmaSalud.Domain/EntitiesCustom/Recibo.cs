using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class Recibo : Entities.Recibo
    {
        public string Cliente { get; set; }

        public List<EntitiesCustom.ReciboDetalle> Items { get; set; }

        public List<Entities.ReciboRetencion> Retenciones { get; set; }
    }
}
