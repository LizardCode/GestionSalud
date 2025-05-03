namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class BalancePatrimonial
    {
        public int Item { get; set; }
        public string Rubro { get; set; }
        public string CodigoIntegracion { get; set; }
        public string NumeroCuenta { get; set; }
        public string Descripcion { get; set; }
        public double? Saldo { get; set; }
        public double? Total { get; set; }
    }
}
