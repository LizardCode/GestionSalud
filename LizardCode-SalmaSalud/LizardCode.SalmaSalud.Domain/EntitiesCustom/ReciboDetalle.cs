using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ReciboDetalle : Entities.ReciboDetalle
    {
        #region Cheque

		public string NroCheque { get; set; }
		public string BancoCheque { get; set; }
		public DateTime? FechaEmision { get; set; }
		public DateTime? FechaVto { get; set; }
		public string FirmanteCheque { get; set; }
		public string CUITFirmante { get; set; }
		public string Endosante1Cheque { get; set; }
		public string CUITEndosante1 { get; set; }
		public string Endosante2Cheque { get; set; }
		public string CUITEndosante2 { get; set; }
		public string Endosante3Cheque { get; set; }
		public string CUITEndosante3 { get; set; }

		#endregion

		#region Transferencia

		public DateTime? FechaTransferencia { get; set; }
		public string NroTransferencia { get; set; }
		public int? IdBancoTranferencia { get; set; }
		public string BancoOrigen { get; set; }

		#endregion

		#region Documento

		public DateTime? FechaDocumento { get; set; }
		public DateTime? FechaVtoDocumento { get; set; }
		public string NroDocumento { get; set; }
		public string FirmanteDocumento { get; set; }
		public string CUITFirmanteDocumento { get; set; }

		#endregion
	}
}
