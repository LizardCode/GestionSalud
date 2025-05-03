using CsvHelper.Configuration.Attributes;
using System;

namespace LizardCode.SalmaSalud.Application.Models.CargaManual
{
    [Delimiter(",")]
    public class CargaManualCSV
    {
        [Name("Fecha")]
        [Format("dd/MM/yyyy")]
        public DateTime Fecha { get; set; }
        [Name("Tipo")]
        public string Tipo { get; set; }
        [Name("Punto de Venta")]
        public string PuntoVenta { get; set; }
        [Name("Número Desde")]
        public string NumeroDesde { get; set; }
        [Name("Número Hasta")]
        public string NumeroHasta { get; set; }
        [Name("Cód. Autorización")]
        public string CAE { get; set; }
        [Name("Tipo Doc. Emisor")]
        public string TipoDocEmisor { get; set; }
        [Name("Nro. Doc. Emisor")]
        public string NroDocEmisor { get; set; }
        [Name("Denominación Emisor")]
        public string DenominacionEmisor { get; set; }
        [Name("Tipo Cambio")]
        public double TipoCambio { get; set; }
        [Name("Moneda")]
        public string Moneda { get; set; }
        [Name("Imp. Neto Gravado")]
        [Default(0)]
        public double NetoGravado { get; set; }
        [Name("Imp. Neto No Gravado")]
        [Default(0)]
        public double NetoNoGravado { get; set; }
        [Name("Imp. Op. Exentas")]
        [Default(0)]
        public double ImpOpExentas { get; set; }
        [Name("IVA")]
        [Default(0)]
        public double IVA { get; set; }
        [Name("Imp. Total")]
        public double Total { get; set; }
    }
}
