using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SdoCtaCtePrvItem
    {
        public int Item { get; set; }
        public int IdProveedor { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Vencimiento { get; set; }
        public int IdComprobante { get; set; }
        public string Sucursal { get; set; }
        public string Numero { get; set; }
        public double NetoGravado { get; set; }
        public double IdAlicuota { get; set; }
        public double Percepciones { get; set; }
        public double Total { get; set; }

    }
}
