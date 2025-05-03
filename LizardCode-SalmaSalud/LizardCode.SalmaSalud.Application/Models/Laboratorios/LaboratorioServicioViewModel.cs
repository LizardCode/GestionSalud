using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Laboratorios
{
    public class LaboratorioServicioViewModel
    {
        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdProveedor { get; set; }

        [RepeaterColumn(Header = "", Position = 2, Hidden = true)]
        public int IdLaboratorioServicio { get; set; }

        //[RepeaterColumn(Header = "#", Position = 3, Width = 40, Readonly = true, Hidden = true)]
        //public int Item { get; set; }

        //[RepeaterColumn(RepeaterColumnType.Input, Header = "Codigo", Width = 150, Position = 4)]
        //[AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        //[RequiredEx]
        //public string Codigo { get; set; }

        [RepeaterColumn(RepeaterColumnType.Input, Header = "Descripcion", Width = 0, Position = 3)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        [RequiredEx]
        public string Descripcion { get; set; }

        [RepeaterColumn(RepeaterColumnType.Input, Header = "Valor $", ControlType = RepeaterColumnType.Currency, Width = 120, Position = 4)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [RequiredEx]
        public double Valor { get; set; }
    }
}