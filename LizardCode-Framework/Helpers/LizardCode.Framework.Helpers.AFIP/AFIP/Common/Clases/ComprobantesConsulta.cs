using System;

namespace LizardCode.Framework.Helpers.AFIP.Common.Clases
{
    public class ComprobantesConsulta
    {
        public string CbteModo { get; set; }
        public long CuitEmisor { get; set; }
        public int PtoVta { get; set; }
        public int CbteTipo { get; set; }
        public long CbteNro { get; set; }
        public DateTime CbteFch { get; set; }
        public double ImpTotal { get; set; }
        public string CodAutorizacion { get; set; }
        public string DocTipoReceptor { get; set; }
        public string DocNroReceptor { get; set; }
    }
}
