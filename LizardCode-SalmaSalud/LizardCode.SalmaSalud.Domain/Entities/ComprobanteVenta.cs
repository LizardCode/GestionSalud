using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ComprobantesVentas")]

    public class ComprobanteVenta
    {
        [Key]
		public int IdComprobanteVenta { get; set; }
		public int IdComprobante { get; set; }
		public int IdSucursal { get; set; }
		public string Sucursal { get; set; }
		public string Numero { get; set; }
		public int? IdEjercicio { get; set; }
		public int IdCliente { get; set; }
		public int IdEmpresa { get; set; }
		public DateTime Fecha { get; set; }
		public DateTime? FechaVto { get; set; }
		public double Subtotal { get; set; }
		public double Total { get; set; }
		public string CAE { get; set; }
		public DateTime? VencimientoCAE { get; set; }
		public int IdUsuario { get; set; }
		public DateTime FechaIngreso { get; set; }
		public int IdTipoComprobante { get; set; }
		public string Moneda { get; set; }
		public double Cotizacion { get; set; }
		public int IdEstadoAFIP { get; set; }
		public string ReferenciaComercial { get; set; }
        public string DescripcionUnica { get; set; }
        public int? IdCondicion { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

        
    }
}
