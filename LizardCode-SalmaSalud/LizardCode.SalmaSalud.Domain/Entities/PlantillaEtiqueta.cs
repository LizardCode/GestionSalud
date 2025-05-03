using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("PlantillasEtiquetas")]
	
	public class PlantillaEtiqueta
	{
		[Key]
		public int IdPlantillaEtiqueta { get; set; }
		public int IdTipoPlantilla { get; set; }
		public string Etiqueta { get; set; }
		public string Descripcion { get; set; }
		public string Ejemplo { get; set; }
		public string Tabla { get; set; }
		public string Campo { get; set; }
	}
}
