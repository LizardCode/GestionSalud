using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class CitiVenta
    {
        public DateTime Fecha { get; set; }
        public string TipoComprobante { get; set; }
        public string PuntoVenta { get; set; }
        public string NroComprobanteDesde { get; set; }
        public string NroComprobanteHasta { get; set; }
        public string CodDocumentoComprador { get; set; }
        public string NroIdentificaiconComprador { get; set; }
        public string ApellidoNombreComprador { get; set; }
        public double ImporteTotal { get; set; }
        public double ImporteConceptos { get; set; }
        public double ImportePercepcion { get; set; }
        public double ImporteOperacioneExentas { get; set; }
        public double PercepcionIVA { get; set; }
        public double PercepcionIBrutos { get; set; }
        public double PercepcionImpuestosMunicipales { get; set; }
        public double ImporteImpuestosInternos { get; set; }
        public string Moneda { get; set; }
        public double TipoCambio { get; set; }
        public int CantidadAlicuotas { get; set; }
        public string CodigoOperacion { get; set; }
        public double OtrosTributos { get; set; }
        public DateTime VtoPago { get; set; }
}
}
