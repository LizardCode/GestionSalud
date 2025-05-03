using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ComprobanteVenta : Entities.ComprobanteVenta
    {
        public string Comprobante { get; set; }
        public string Cliente { get; set; }
        public IList<Entities.ComprobanteVentaItem> Items { get; set; }

    }
}
