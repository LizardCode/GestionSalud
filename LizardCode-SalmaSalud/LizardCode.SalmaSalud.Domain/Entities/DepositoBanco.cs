using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("DepositosBanco")]
	
	public class DepositoBanco
	{
		[Key]
		public int IdDepositoBanco { get; set; }
		public DateTime Fecha { get; set; }
		public int IdEjercicio { get; set; }
		public int IdEmpresa { get; set; }
		public int IdBanco { get; set; }
		public string Descripcion { get; set; }
		public double Importe { get; set; }		
		public int IdUsuario { get; set; }
		public DateTime FechaIngreso { get; set; }
		public string Moneda { get; set; }
		public double Cotizacion { get; set; }

		[SoftDelete((int)EstadoRegistro.Eliminado)]
		public int IdEstadoRegistro { get; set; }
	}
}
