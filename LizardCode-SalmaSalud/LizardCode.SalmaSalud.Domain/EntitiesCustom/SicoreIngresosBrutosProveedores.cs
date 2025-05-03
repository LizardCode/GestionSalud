using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SicoreIngresosBrutosProveedores
    {
        public string CUIT { get; set; }
        public DateTime FechaRetencion { get; set; }
        public string Sucursal { get; set; }
        public string NroRetencion { get; set; }
        public double ImporteRetencion   { get; set; }
        public string CodigoOperacion { get; set; }
    }
}
