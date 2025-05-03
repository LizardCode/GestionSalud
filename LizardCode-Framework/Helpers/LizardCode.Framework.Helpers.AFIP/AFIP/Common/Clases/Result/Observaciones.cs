namespace LizardCode.Framework.Helpers.AFIP.Common
{
    public class Observaciones
    {
        public int Codigo { get; set; }
        public string MensajeObservacion { get; set; }

        public Observaciones()
        {
            //
        }

        public Observaciones(int codigo, string mensajeObservacion)
        {
            Codigo = codigo;
            MensajeObservacion = mensajeObservacion;
        }
    }
}
