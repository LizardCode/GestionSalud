namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class BalanceSumSdo
    {
        public int Item { get; set; }
        public int? IdCuentaContable { get; set; }
        public string CodigoIntegracion { get; set; }
        public string Descripcion { get; set; }
        public double? Debe { get; set; }
        public double? Haber { get; set; }
        public double? Deudor { get; set; }
        public double? Acredor { get; set; }
    }
}
