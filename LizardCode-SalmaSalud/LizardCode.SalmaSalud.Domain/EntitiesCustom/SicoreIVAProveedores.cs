using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SicoreIVAProveedores
    {
        public string CodigoComprobante { get; set; }
        public DateTime FechaEmisionComprobante { get; set; }
        public string NroComprobante { get; set; }
        public double ImporteNetoGravado { get; set; }
        public string CodImpuesto { get; set; }
        public string Regimen { get; set; }
        public string CodigoOperacion { get; set; }
        public double BaseImponible { get; set; }
        public DateTime FechaEmisionRetencion { get; set; }
        public string CodigoCondicion { get; set; }
        public string CodSujetosSuspendidos { get; set; }
        public double ImporteRetencion { get; set; }
        public string PorcentajeExclusion { get; set; }
        public string FechaEmisionBoletin { get; set; }
        public string TipoDocumentoDelRetenido { get; set; }
        public string NroDocumentoDelRetenido { get; set; }
        public string NroCertificadoOriginal { get; set; }
        public string Espacios { get; set; }
    }
}
