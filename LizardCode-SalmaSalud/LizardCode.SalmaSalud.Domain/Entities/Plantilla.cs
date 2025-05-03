using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Plantillas")]
	
	public class Plantilla
	{
		[Key]
		public int IdPlantilla { get; set; }
		public int IdEmpresa { get; set; }
		public string Descripcion { get; set; }
		public int IdTipoPlantilla { get; set; }
		public byte[] PDF { get; set; }
		public double Top { get; set; }
		public double Left { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }

		[SoftDelete((int)EstadoRegistro.Eliminado)]
		public int IdEstadoRegistro { get; set; }

        public byte[] JSON { get; set; }
    }
}
