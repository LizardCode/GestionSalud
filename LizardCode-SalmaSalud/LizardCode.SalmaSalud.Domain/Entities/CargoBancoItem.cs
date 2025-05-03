using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CargosBancoItems")]

    public class CargoBancoItem
	{
        [ExplicitKey]
		public int IdCargoBanco { get; set; }
		[ExplicitKey]
		public int Item { get; set; }
		public int IdCuentaContable { get; set; }
		public string Detalle { get; set; }
		public double Importe { get; set; }
		public double Alicuota { get; set; }
		public double Total { get; set; }

		[SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
