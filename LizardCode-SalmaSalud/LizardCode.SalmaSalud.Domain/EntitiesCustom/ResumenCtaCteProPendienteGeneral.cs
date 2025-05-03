using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
	public class ResumenCtaCteProPendienteGeneral
	{
		public string Proveedor { get; set; }
		public string CUIT { get; set; }
		public DateTime? Fecha { get; set; }
		public DateTime? FechaVto { get; set; }
		public string Comprobante { get; set; }
		public string Sucursal { get; set; }
		public string Numero { get; set; }
		public double Saldo { get; set; }
	}
}
