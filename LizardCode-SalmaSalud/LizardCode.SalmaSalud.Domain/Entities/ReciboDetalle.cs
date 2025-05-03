using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("RecibosDetalle")]
	
	public class ReciboDetalle
    {
		[Key]
		public int IdReciboDetalle { get; set; }
		public int IdRecibo { get; set; }
		public int IdTipoCobro { get; set; }
		public string Descripcion { get; set; }
		public double Importe { get; set; }
		public int? IdCheque { get; set; }
		public int? IdTransferencia { get; set; }
		public int? IdDocumento { get; set; }

	}
}
