using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.Recibos
{
    public class RecibosDetalle
    {
        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdRecibo { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Descripción", Width = 0, Position = 2)]
        public string Descripcion { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Importe", Width = 110, Position = 3)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }

        #region Modal de Tipo de Cobro

        [RequiredEx]
        [MasterDetailColumn(Header = "Tipo de Cobro", Hidden = true, Position = 4)]
        public int IdTipoCobro { get; set; }
        public string TipoCobro { get; set; }

        #region Cheque (Position 5/16)

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha de Emisión", Hidden = true, Position = 5)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaEmision { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha de Vencimiento", Hidden = true, Position = 6)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaVto { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Banco", Hidden = true, Position = 7)]
        public string BancoCheque { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[10]", numericOnly: true)]
        [MasterDetailColumn(Header = "Número de Cheque", Hidden = true, Position = 8)]
        public string NroCheque { get; set; }

        [MasterDetailColumn(Header = "Firmante", Hidden = true, Position = 9)]
        public string FirmanteCheque { get; set; }

        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,8,1]", delimiters: "-", numericOnly: true)]
        [MasterDetailColumn(Header = "CUIT", Hidden = true, Position = 10)]
        public string CUITFirmante { get; set; }

        [MasterDetailColumn(Header = "Endosante 1", Hidden = true, Position = 11)]
        public string Endosante1Cheque { get; set; }

        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,8,1]", delimiters: "-", numericOnly: true)]
        [MasterDetailColumn(Header = "CUIT", Hidden = true, Position = 12)]
        public string CUITEndosante1 { get; set; }

        [MasterDetailColumn(Header = "Endosante 2", Hidden = true, Position = 13)]
        public string Endosante2Cheque { get; set; }

        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,8,1]", delimiters: "-", numericOnly: true)]
        [MasterDetailColumn(Header = "CUIT", Hidden = true, Position = 14)]
        public string CUITEndosante2 { get; set; }

        [MasterDetailColumn(Header = "Endosante 3", Hidden = true, Position = 15)]
        public string Endosante3Cheque { get; set; }

        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,8,1]", delimiters: "-", numericOnly: true)]
        [MasterDetailColumn(Header = "CUIT", Hidden = true, Position = 16)]
        public string CUITEndosante3 { get; set; }

        #endregion

        #region Transferencia (Position 17/21)

        [MasterDetailColumn(Header = "Número de Tranferencia", Hidden = true, Position = 17)]
        public string NroTransferencia { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Banco", Hidden = true, Position = 18)]
        public int? IdBancoTranferencia { get; set; }
        public string BancoTranferencia { get; set; }

        [MasterDetailColumn(Header = "Bando de Origen", Hidden = true, Position = 19)]
        public string BancoOrigen { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha", Hidden = true, Position = 20)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaTransferencia { get; set; }

        #endregion

        #region Documento (Position 21/25)

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha de Emisión", Hidden = true, Position = 21)]
        [MasterDetailFormatAttribute(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaDocumento { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha de Vencimiento", Hidden = true, Position = 22)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaVtoDocumento { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[10]", numericOnly: true)]
        [MasterDetailColumn(Header = "Número de Documento", Hidden = true, Position = 23)]
        public string NroDocumento { get; set; }

        [MasterDetailColumn(Header = "Firmante", Hidden = true, Position = 24)]
        public string FirmanteDocumento{ get; set; }

        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,8,1]", delimiters: "-", numericOnly: true)]
        [MasterDetailColumn(Header = "CUIT", Hidden = true, Position = 25)]
        public string CUITFirmanteDocumento { get; set; }

        #endregion

        #endregion
    }
}
