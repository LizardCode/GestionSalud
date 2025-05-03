using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class OrdenPago : Entities.OrdenPago
    {
        public string Proveedor { get; set; }

        public IList<Entities.OrdenPagoDetalle> Items { get; set; }
    }
}
