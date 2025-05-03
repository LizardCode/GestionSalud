namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SubdiarioPagosDetalle : SubdiarioPagos
    {
        public string CodigoCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public double Importe { get; set; }
    }
}
