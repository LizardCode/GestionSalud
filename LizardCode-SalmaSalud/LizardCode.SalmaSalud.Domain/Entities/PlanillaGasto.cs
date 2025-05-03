using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("PlanillaGastos")]

    public class PlanillaGastos
	{
        [Key]
		public int IdPlanillaGastos { get; set; }
		public int IdEmpresa { get; set; }
		public int IdEjercicio { get; set; }
		public DateTime Fecha { get; set; }
		public string Descripcion { get; set; }
		public double ImporteTotal { get; set; }
		public int IdEstadoPlanilla { get; set; }
		public int IdUsuario { get; set; }
		public DateTime FechaIngreso { get; set; }
		public int IdCuentaContable { get; set; }
		public string Moneda { get; set; }

		[SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
