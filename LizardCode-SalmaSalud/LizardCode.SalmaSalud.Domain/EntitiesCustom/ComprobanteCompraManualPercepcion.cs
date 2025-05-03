namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ComprobanteCompraManualPercepcion
    {
		public int IdComprobanteCompra { get; set; }
		public int Item { get; set; }
		public int IdCuentaContable { get; set; }
		public string CuentaContable { get; set; }
		public double Importe { get; set; }
	}
}
