using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SubdiarioCompras
    {
        public int IdComprobanteCompra { get; set; }
        public DateTime Fecha { get; set; }
        public string Comprobante { get; set; }
        public string Sucursal { get; set; }
        public string Numero { get; set; }
        public string Cliente { get; set; }
        public string CUIT { get; set; }
        public string TipoIVA { get; set; }
        public string CentroCostos { get; set; }
        public double Total { get; set; }
    }
}
