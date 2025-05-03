using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ComprobantesComprasAFIP")]

    public class ComprobanteCompraAFIP
    {
        [Key]
		public int IdComprobanteCompraAFIP { get; set; }
		public int IdComprobanteCompra { get; set; }
		public string Estado { get; set; }
		public string Request { get; set; }
		public string Response { get; set; }
		public DateTime FechaRequest { get; set; }

	}
}
