using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.SaldoInicioBanco
{
    public class SaldoInicioBancoCheques
    {
        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdSaldoInicioBanco { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Descripción", Width = 0, Position = 2)]
        public string Descripcion { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Importe", Width = 110, Position = 3)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }

        #region Modal de Tipo de Pago

        [RequiredEx]
        [MasterDetailColumn(Header = "Tipo de Cheque", Hidden = true, Position = 4)]
        public int IdTipoCheque { get; set; }
        public string TipoCheque { get; set; }

        #region Cheque Común (Position 5/7)

        [RequiredEx]
        [MasterDetailColumn(Header = "Banco", Hidden = true, Position = 5)]
        public int? IdBancoChequeComun { get; set; }
        public string BancoChequeComun { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[10]", numericOnly: true)]
        [MasterDetailColumn(Header = "Número de Cheque", Hidden = true, Position = 6)]
        public string NumeroChequeComun { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha", Hidden = true, Position = 7)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaChequeComun { get; set; }

        #endregion

        #region Cheque Diferido (Position 8/11)

        [RequiredEx]
        [MasterDetailColumn(Header = "Banco", Hidden = true, Position = 8)]
        public int? IdBancoChequeDiferido { get; set; }
        public string BancoChequeDiferido { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[10]", numericOnly: true)]
        [MasterDetailColumn(Header = "Número de Cheque", Hidden = true, Position = 9)]
        public string NumeroChequeDiferido { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha", Hidden = true, Position = 10)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaChequeDiferido { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha Diferido", Hidden = true, Position = 11)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaDiferidoChequeDiferido { get; set; }

        #endregion

        #region E-Cheque Común (Position 12/14)

        [RequiredEx]
        [MasterDetailColumn(Header = "Banco", Hidden = true, Position = 12)]
        public int? IdBancoEChequeComun { get; set; }
        public string BancoEChequeComun { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[10]", numericOnly: true)]
        [MasterDetailColumn(Header = "Número de Cheque", Hidden = true, Position = 13)]
        public string NumeroEChequeComun { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha", Hidden = true, Position = 14)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaEChequeComun { get; set; }

        #endregion

        #region E-Cheque Diferido (Position 15/18)

        [RequiredEx]
        [MasterDetailColumn(Header = "Banco", Hidden = true, Position = 15)]
        public int? IdBancoEChequeDiferido { get; set; }
        public string BancoEChequeDiferido { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[10]", numericOnly: true)]
        [MasterDetailColumn(Header = "Número de Cheque", Hidden = true, Position = 16)]
        public string NumeroEChequeDiferido { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha", Hidden = true, Position = 17)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaEChequeDiferido { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha Diferido", Hidden = true, Position = 18)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaDiferidoEChequeDiferido { get; set; }

        #endregion

        #region Cheque Terceros (Position 19/23)

        [RequiredEx]
        [MasterDetailColumn(Header = "Banco", Hidden = true, Position = 19)]
        public int? IdChequeTerceros { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Banco", Hidden = true, Position = 20)]
        public string BancoChequeTerceros { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[10]", numericOnly: true)]
        [MasterDetailColumn(Header = "Número de Cheque", Hidden = true, Position = 21)]
        public string NumeroChequeTerceros { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha", Hidden = true, Position = 22)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaChequeTerceros { get; set; }

        [RequiredEx]
        [MasterDetailColumn(Header = "Fecha Diferido", Hidden = true, Position = 23)]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Date)]
        public DateTime? FechaDiferidoChequeTerceros { get; set; }

        #endregion

        #endregion
    }
}
