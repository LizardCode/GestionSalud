using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ARBA")]
	
	public class ARBA
    {
		public string Regimen { get; set; }
		public string FechaPublicacion { get; set; }
		public string FechaVigDesde { get; set; }
		public string FechaVigHasta { get; set; }
		public string CUIT { get; set; }
		public string Tipo { get; set; }
		public string MarcaAlicuota { get; set; }
		public string MarcaCambio { get; set; }
		public double Alicuota { get; set; }
		public string NroGrupo { get; set; }
	}
}
