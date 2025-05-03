using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SubdiarioPagos
    {
        public int IdOrdenPago { get; set; }
        public DateTime Fecha { get; set; }
        public string Comprobante { get; set; }
        public string Numero { get; set; }
        public string Proveedor { get; set; }
        public string CUIT { get; set; }
        public double Total { get; set; }
    }
}
