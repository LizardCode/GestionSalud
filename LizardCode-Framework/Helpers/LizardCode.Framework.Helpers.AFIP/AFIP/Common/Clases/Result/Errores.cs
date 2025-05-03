namespace LizardCode.Framework.Helpers.AFIP.Common
{
    public class Errores
    {
        public int Codigo { get; set; }
        public string MensajeError { get; set; }

        public Errores()
        {
            //
        }

        public Errores(int codigo, string mensajeError)
        {
            Codigo = codigo;
            MensajeError = mensajeError;
        }
    }
}
