using System.Collections.Generic;

namespace LizardCode.Framework.Helpers.AFIP.Common
{
    public class CAE
    {
        public string NroCAE { get; set; }
        public string CAEFchVto { get; set; }
        public string Resultado { get; set; }
        public string Error { get; set; }
        public string Observacion { get; set; }

        public string XMLRequest { get; set; }
        public string XMLResponse { get; set; }

        public List<Observaciones> Observaciones { get; set; }
        public List<Errores> Errores { get; set; }

    }
}
