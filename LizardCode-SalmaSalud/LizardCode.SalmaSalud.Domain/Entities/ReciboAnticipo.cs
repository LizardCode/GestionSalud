using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("RecibosAnticipos")]
	
	public class ReciboAnticipo
	{
		[Key]
		public int IdReciboAnticipo { get; set; }
		public int IdRecibo { get; set; }
		public int? IdAnticipo { get; set; }
		public double Importe { get; set; }
	}
}
