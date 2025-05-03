using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class DepositoBancoDetalle : Entities.DepositoBancoDetalle
	{
		#region Cheque

		public int IdBanco { get; set; }
		public string NroCheque { get; set; }
		public string BancoCheque { get; set; }
		public DateTime? FechaEmision { get; set; }
		public DateTime? FechaVto { get; set; }

		#endregion

		#region Transferencia

		public DateTime? FechaTransferencia { get; set; }
		public string NroTransferencia { get; set; }
		public int? IdBancoTranferencia { get; set; }

		#endregion

	}
}
