namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ComprobanteCompraManualItem
    {
        public int Item { get; set; }
        public string Descripcion { get; set; }
        public int IdCuentaContable { get; set; }
        public string CuentaContable { get; set; }
        public int IdAlicuota { get; set; }
        public string Alicuota { get; set; }
        public double Importe { get; set; }

    }
}
