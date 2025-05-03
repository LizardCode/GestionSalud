using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class OrdenPagoDetalle : Entities.OrdenPagoDetalle
	{
        #region Cheque

        public int? IdBancoCheque { get; set; }
        public string BancoCheque { get; set; }
        public string NumeroCheque { get; set; }
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaVto { get; set; }

        #endregion

        #region Transferencia

        public int? IdBancoTranferencia { get; set; }
        public string BancoTranferencia { get; set; }
        public DateTime? FechaTransferencia { get; set; }
        public string NumeroTransferencia { get; set; }

        #endregion

    }
}
