using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("AsientosDetalle")]
	
	public class AsientoDetalle
	{
		[ExplicitKey]
		public int IdAsiento { get; set; }
		[ExplicitKey]
		public int Item { get; set; }
		public int IdCuentaContable { get; set; }
		public string Detalle { get; set; }
		public double Debitos { get; set; }
		public double Creditos { get; set; }
	}
}
