using System;
using System.Collections.Generic;

namespace LizardCode.Framework.Helpers.AFIP.Common.Clases
{
    public class Comprobantes
    {
        public int CbteTipo { get; set; }
        public int PtoVta { get; set; }
        public int Concepto { get; set; }
        public int DocTipo { get; set; }
        public long DocNro { get; set; }
        public long CbteDesde { get; set; }
        public long CbteHasta { get; set; }
        public DateTime CbteFch { get; set; }
        public DateTime? FchVtoPago { get; set; }
        public double ImpTotal { get; set; }
        public double ImpTotConc { get; set; }
        public double ImpNeto { get; set; }
        public double ImpOpEx { get; set; }
        public double ImpTrib { get; set; }
        public double ImpIVA { get; set; }
        public DateTime FchServDesde { get; set; }
        public DateTime FchServHasta { get; set; }
        public string MonId { get; set; }
        public double MonCotiz { get; set; }

        public DateTime? PeriodoAsocFchServDesde { get; set; }
        public DateTime? PeriodoAsocFchServHasta { get; set; }

        public List<Tributos> TipoTrib { get; set; }
        public List<Alicuotas> TipoAlic { get; set; }
        public List<Opcionales> Opcionales { get; set; }
        public List<ComprobantesAsociados> ComprobantesAsociados { get; set; }
    }
}
