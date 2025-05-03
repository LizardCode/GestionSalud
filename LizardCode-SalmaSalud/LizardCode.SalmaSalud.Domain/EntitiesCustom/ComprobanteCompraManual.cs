using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ComprobanteCompraManual : Entities.ComprobanteCompra
    {
        public string Comprobante { get; set; }
        public string Proveedor { get; set; }
        public IList<ComprobanteCompraManualItem> Items { get; set; }
        public IList<ComprobanteCompraManualPercepcion> ListaPercepciones { get; set; }
        public IList<OrdenPagoGrilla> Pagos { get; set; }

    }
}
