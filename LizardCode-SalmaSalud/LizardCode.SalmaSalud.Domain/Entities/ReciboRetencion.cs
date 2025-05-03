using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("RecibosRetenciones")]
	
	public class ReciboRetencion
    {
		[Key]
		public int IdReciboRetencion { get; set; }
		public int IdRecibo { get; set; }
		public int IdCategoria { get; set; }
		public string Categoria { get; set; }
        public int IdCuentaContable { get; set; }
		public DateTime Fecha { get; set; }
		public string NroRetencion { get; set; }
		public double BaseImponible { get; set; }
		public double Importe { get; set; }

	}
}
