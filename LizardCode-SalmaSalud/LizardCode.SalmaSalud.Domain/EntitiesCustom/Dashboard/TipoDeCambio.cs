using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom.Dashboard
{
    public class TipoDeCambio
    {
        public string Simbolo { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha{ get; set; }
        public double Cotizacion { get; set; }

    }
}
