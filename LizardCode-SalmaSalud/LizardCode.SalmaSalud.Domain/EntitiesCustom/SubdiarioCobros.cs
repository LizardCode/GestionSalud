using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SubdiarioCobros
    {
        public int IdRecibo { get; set; }
        public DateTime Fecha { get; set; }
        public string Comprobante { get; set; }
        public string Numero { get; set; }
        public string Cliente { get; set; }
        public string CUIT { get; set; }
        public double Total { get; set; }
    }
}
