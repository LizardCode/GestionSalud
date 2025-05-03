using System;

namespace LizardCode.SalmaSalud.Application.Models.OrdenesPago
{
    public class OrdenesPagoRetencion
    {
        public int IdOrdenPago { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoRetencion { get; set; }
        public string NroRetencion { get; set; }
        public double BaseImponible { get; set; }
        public double Importe { get; set; }

    }
}
