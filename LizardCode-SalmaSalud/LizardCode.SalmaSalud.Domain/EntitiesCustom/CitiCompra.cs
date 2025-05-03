using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class CitiCompra
    {
        public DateTime Fecha { get; set; }
        public string TipoComprobante { get; set; }
        public string PuntoVenta { get; set; }
        public string NroComprobante { get; set; }
        public string NroDespachoImportacion { get; set; }
        public string CodDocumentoVendedor { get; set; }
        public string NroIdentificaiconVendedor { get; set; }
        public string ApellidoNombreVendedor { get; set; }
        public double ImporteTotal { get; set; }
        public double ImporteConceptos { get; set; }
        public double ImporteOperacioneExentas { get; set; }
        public double PercepcionIVA { get; set; }
        public double PercepcionImpuestosNacionales { get; set; }
        public double PercepcionIBrutos { get; set; }
        public double PercepcionImpuestosMunicipales { get; set; }
        public double ImporteImpuestosInternos { get; set; }
        public string Moneda { get; set; }
        public double TipoCambio { get; set; }
        public int CantidadAlicuotas { get; set; }
        public string CodigoOperacion { get; set; }
        public double CreditoFiscalImputable { get; set; }
        public double OtrosTributos { get; set; }
        public string CUITEmisorCorredor { get; set; }
        public string DenominacionEmisorCorredor { get; set; }
        public double IVAComision { get; set; }
}
}
