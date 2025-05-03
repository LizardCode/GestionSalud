using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class DetalleFacturas
    {
        public string Comprobante { get; set; }
        public DateTime Fecha { get; set; }
        public double ImporteFacturado { get; set; }
    }
}
