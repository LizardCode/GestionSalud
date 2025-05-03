using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CargosBanco")]

    public class CargoBanco
	{
        [Key]
		public int IdCargoBanco { get; set; }
		public int IdEjercicio { get; set; }
		public DateTime Fecha { get; set; }
		public string Descripcion { get; set; }
		public DateTime FechaReal { get; set; }
		public int IdEmpresa { get; set; }
		public int IdBanco { get; set; }
		public double Total { get; set; }
		public int IdUsuario { get; set; }
		public DateTime FechaIngreso { get; set; }

		[SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
