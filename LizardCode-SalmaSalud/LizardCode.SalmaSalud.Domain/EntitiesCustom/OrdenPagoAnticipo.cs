namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class OrdenPagoAnticipo
    {
		public bool Seleccionar { get; set; }
		public int IdOrdenPago { get; set; }
		public string Descripcion { get; set; }
		public string Importe { get; set; }
		public string Saldo { get; set; }

	}
}
