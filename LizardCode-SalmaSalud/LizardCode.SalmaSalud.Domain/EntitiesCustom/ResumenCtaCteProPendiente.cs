using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ResumenCtaCteProPendiente
    {
        public DateTime? Fecha { get; set; }
        public DateTime? FechaVto { get; set; }
        public int IdComprobante { get; set; }
        public string Comprobante { get; set; }
        public bool EsCredito { get; set; }
        public string Sucursal { get; set; }
        public string Numero { get; set; }
        public double Saldo { get; set; }
    }
}
