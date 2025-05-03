using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("AGIP")]
	
	public class AGIP
    {
		public string FechaPublicacion { get; set; }
		public string FechaVigDesde { get; set; }
		public string FechaVigHasta { get; set; }
		public string CUIT { get; set; }
		public string Tipo { get; set; }
		public string AltaSujeto { get; set; }
		public string MarcaAlicuota { get; set; }
		public double AliPercepcion { get; set; }
		public double AliRetencion { get; set; }
		public string NroGrupoPercepcion { get; set; }
		public string NroGrupoRetencion { get; set; }
		public string RazonSocial { get; set; }
	}
}
