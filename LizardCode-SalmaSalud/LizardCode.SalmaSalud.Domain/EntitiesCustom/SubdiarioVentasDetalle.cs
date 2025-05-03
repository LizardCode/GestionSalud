namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SubdiarioVentasDetalle : SubdiarioVentas
    {
        public string CodigoCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public double Importe { get; set; }
    }
}
