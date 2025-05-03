using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ComprobanteCompra : Entities.ComprobanteCompra
    {
        public string Comprobante { get; set; }
        public string Proveedor { get; set; }
        public IList<ComprobanteCompraItem> Items { get; set; }
        public bool Pagada { get; set; }
    }
}
