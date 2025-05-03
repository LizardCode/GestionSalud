using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Cheques")]

    public class Cheque
    {
        [Key]
		public int IdCheque { get; set; }
		public int IdEmpresa { get; set; }
		public int IdTipoCheque { get; set; }
		public string NroCheque { get; set; }
		public int? IdBanco { get; set; }
		public string Banco { get; set; }
		public DateTime? FechaEmision { get; set; }
		public DateTime? FechaVto { get; set; }
		public double Importe { get; set; }
		public int IdEstadoCheque { get; set; }
		public string Firmante { get; set; }
		public string CUITFirmante { get; set; }
		public string Endosante1 { get; set; }
		public string CUITEndosante1 { get; set; }
		public string Endosante2 { get; set; }
		public string CUITEndosante2 { get; set; }
		public string Endosante3 { get; set; }
		public string CUITEndosante3 { get; set; }

		[SoftDelete((int)EstadoRegistro.Eliminado)]
		public int IdEstadoRegistro { get; set; }

	}
}
