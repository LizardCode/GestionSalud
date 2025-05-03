using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ComprobantesVentasAFIP")]

    public class ComprobanteVentaAFIP
    {
        [Key]
		public int IdComprobanteVentaAFIP { get; set; }
		public int IdComprobanteVenta { get; set; }
		public string Estado { get; set; }
		public string CAE { get; set; }
		public DateTime? VencimientoCAE { get; set; }
		public string Request { get; set; }
		public string Response { get; set; }
		public DateTime FechaRequest { get; set; }

	}
}
