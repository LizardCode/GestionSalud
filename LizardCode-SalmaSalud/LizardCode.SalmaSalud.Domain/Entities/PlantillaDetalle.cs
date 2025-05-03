using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("PlantillaDetalle")]
	
	public class PlantillaDetalle
	{
		[Key]
		public int IdPlantillaDetalle { get; set; }
		public int IdPlantilla { get; set; }
		public int IdPlantillaEtiqueta { get; set; }
		public double Top { get; set; }
		public double Left { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
	}
}
