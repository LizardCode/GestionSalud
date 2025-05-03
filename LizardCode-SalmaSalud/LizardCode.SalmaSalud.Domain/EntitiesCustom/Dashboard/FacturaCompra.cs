using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom.Dashboard
{
    public class FacturaCompra
    {
        public DateTime Fecha { get; set; }
        public string Comprobante { get; set; }
        public double Importe { get; set; }

    }
}
