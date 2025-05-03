using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Bancos
{
    public class BancosViewModel
    {
        public int IdBanco { get; set; }

        [RequiredEx]
        public int IdCuentaContable { get; set; }

        [RequiredEx]
        [StringLengthEx(45)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        public string Descripcion { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,8,1]", numericOnly: true)]
        public string CUIT { get; set; }

        [RequiredEx]
        [StringLengthEx(45)]
        public string NroCuenta { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[22]", numericOnly: true)]
        public string CBU { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double SaldoDescubierto { get; set; }

        [RequiredEx] 
        public int IdMoneda { get; set; }

        public int? IdProveedor { get; set; }

        public bool EsDefault { get; set; }


        public SelectList MaestroMonedas { get; set; }

        public SelectList MaestroProveedores { get; set; }

        public SelectList MaestroCuentasContables { get; set; }

    }
}