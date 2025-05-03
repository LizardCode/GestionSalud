using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CondicionVentasCompras")]
	
	public class CondicionVentaCompra
	{
		[Key]
		public int IdCondicion { get; set; }
		public string Descripcion { get; set; }
		public int Dias { get; set; }
		public int IdTipoCondicion { get; set; }

		[SoftDelete((int)EstadoRegistro.Eliminado)]
		public int IdEstadoRegistro { get; set; }
	}
}
