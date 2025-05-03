namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SubdiarioCobrosDetalle : SubdiarioCobros
    {
        public string CodigoCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public double Importe { get; set; }
    }
}
