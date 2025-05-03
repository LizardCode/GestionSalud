using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SicorePercepcionIngresosBrutosProveedores
    {
        public string Jurisdiccion { get; set; }
        public string CUIT { get; set; }
        public DateTime FechaPercepcion { get; set; }
        public string Sucursal { get; set; }
        public string NroPercepcion { get; set; }
        public string CodigoComprobante { get; set; }
        public string LetraComprobante { get; set; }
        public double ImportePercepcion { get; set; }
    }
}
