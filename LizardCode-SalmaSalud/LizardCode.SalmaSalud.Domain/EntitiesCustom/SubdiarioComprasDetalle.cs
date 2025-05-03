namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SubdiarioComprasDetalle : SubdiarioCompras
    {
        public string CodigoCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public double Importe { get; set; }
    }
}
