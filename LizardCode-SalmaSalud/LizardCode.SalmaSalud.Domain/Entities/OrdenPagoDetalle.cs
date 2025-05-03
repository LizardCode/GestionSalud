using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("OrdenesPagoDetalle")]
	
	public class OrdenPagoDetalle
    {
		[Key]
		public int IdOrdenPagoDetalle { get; set; }
		public int IdOrdenPago { get; set; }
		public int IdTipoPago { get; set; }
		public string Descripcion { get; set; }
		public double Importe { get; set; }
		public int? IdCheque { get; set; }
		public int? IdTransferencia { get; set; }
        public int? IdCuentaContable { get; set; }
    }
}
