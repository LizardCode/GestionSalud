using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("DepositosBancoDetalle")]
	
	public class DepositoBancoDetalle
	{
		[Key]
		public int IdDepositoBancoDetalle { get; set; }
		public int IdDepositoBanco { get; set; }
		public int IdTipoDeposito { get; set; }
		public string Descripcion { get; set; }
		public double Importe { get; set; }
		public int? IdCheque { get; set; }
		public int? IdTransferencia { get; set; }
	}
}
