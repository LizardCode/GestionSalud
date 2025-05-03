using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class RetencionPercepcion
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Comprobante { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public string NumeroComprobante { get; set; }
        public double BaseImponible { get; set; }
        public double Importe { get; set; }

    }
}
