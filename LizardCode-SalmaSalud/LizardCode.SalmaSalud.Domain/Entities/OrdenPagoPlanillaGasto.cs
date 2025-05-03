using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("OrdenesPagoPlanillaGastos")]
	
	public class OrdenPagoPlanillaGasto
	{
		[Key]
		public int IdOrdenPagoPlanillaGastos { get; set; }
		public int IdOrdenPago { get; set; }
		public int IdPlanillaGastos { get; set; }
	}
}
