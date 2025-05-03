using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Financiadores
{
    public class FinanciadorPrestacionViewModel
    { 
        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdFinanciador { get; set; }

        [RepeaterColumn(Header = "", Position = 2, Hidden = true)]
        public int IdFinanciadorPrestacion { get; set; }

        [RepeaterColumn(Header = "#", Position = 3, Width = 40, Readonly = true, Hidden = true)]
        public int Item { get; set; }

        [RepeaterColumn(RepeaterColumnType.Input, Header = "Codigo", Width = 150, Position = 4)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        [RequiredEx]
        public string Codigo { get; set; }

        [RepeaterColumn(RepeaterColumnType.Input, Header = "Descripcion", Width = 375, Position = 5)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        [RequiredEx]
        public string Descripcion { get; set; }

        [RepeaterColumn(RepeaterColumnType.Input, Header = "Total", ControlType = RepeaterColumnType.Currency, Width = 100, Position = 6)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [RequiredEx]
        public double Valor { get; set; }
    }
}
