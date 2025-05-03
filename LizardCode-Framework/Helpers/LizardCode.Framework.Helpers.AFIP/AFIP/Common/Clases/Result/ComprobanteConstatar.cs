using System.Collections.Generic;

namespace LizardCode.Framework.Helpers.AFIP.Common.Clases
{
    public class ComprobanteConstatar
    {
       public string CbteModo { get; set; }
       public long CuitEmisor { get; set; }
       public int PtoVta { get; set; }
       public int CbteTipo { get; set; }
       public long CbteNro { get; set; }
       public string CbteFch { get; set; }
       public double ImpTotal { get; set; }
       public string CodAutorizacion { get; set; }
       public string DocTipoReceptor { get; set; }
       public string DocNroReceptor { get; set; }
        public string Resultado { get; set; }
        public string Error { get; set; }
        public string Observacion { get; set; }

        public string XMLRequest { get; set; }
        public string XMLResponse { get; set; }

        public List<Observaciones> Observaciones { get; set; }
       public List<Errores> Errores { get; set; }

    }
}
