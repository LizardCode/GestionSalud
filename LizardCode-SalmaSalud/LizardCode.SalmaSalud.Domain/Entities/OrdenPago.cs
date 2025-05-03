using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("OrdenesPago")]
	
	public class OrdenPago
    {
		[Key]
		public int IdOrdenPago { get; set; }
		public DateTime Fecha { get; set; }
		public int IdTipoOrdenPago { get; set; }
		public int? IdEjercicio { get; set; }
		public int IdEmpresa { get; set; }
		public int? IdProveedor { get; set; }
        public int? IdCuentaContable { get; set; }
        public string Descripcion { get; set; }
		public double Importe { get; set; }
		public string Moneda { get; set; }
        public string MonedaPago { get; set; }
        public double Cotizacion { get; set; }
        public double Total { get; set; }
        public int IdUsuario { get; set; }
		public DateTime FechaIngreso { get; set; }
		public int IdEstadoOrdenPago { get; set; }

		[SoftDelete((int)EstadoRegistro.Eliminado)]
		public int IdEstadoRegistro { get; set; }
	}
}
