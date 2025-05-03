using System.Collections.Generic;

namespace LizardCode.Framework.Helpers.AFIP.Common.Clases
{
    public class Padron
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string CodigoPostal { get; set; }
        public List<int> Impuestos { get; set; }
        public string Errores { get; set; }

    }
}
