using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Recibos")]
	
	public class Recibo
    {
		[Key]
		public int IdRecibo { get; set; }
		public DateTime Fecha { get; set; }
		public int? IdEjercicio { get; set; }
		public int IdTipoRecibo { get; set; }
		public int IdEmpresa { get; set; }
		public int IdCliente { get; set; }
		public string Descripcion { get; set; }
		public double Importe { get; set; }
		public string Moneda { get; set; }
        public string MonedaCobro { get; set; }
        public double Cotizacion { get; set; }
        public double Total { get; set; }
        public int IdUsuario { get; set; }
		public DateTime FechaIngreso { get; set; }
		public int IdEstadoRecibo { get; set; }

		[SoftDelete((int)EstadoRegistro.Eliminado)]
		public int IdEstadoRegistro { get; set; }
	}
}
