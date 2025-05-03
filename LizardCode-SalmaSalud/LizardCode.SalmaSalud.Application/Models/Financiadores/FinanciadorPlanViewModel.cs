using iTextSharp.text;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Financiadores
{
    public class FinanciadorPlanViewModel
    {
        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdFinanciador { get; set; }

        [RepeaterColumn(Header = "", Position = 2, Hidden = true)]
        public int IdFinanciadorPlan { get; set; }

        [RepeaterColumn(Header = "#", Position = 3, Width = 40, Readonly = true, Hidden = true)]
        public int Item { get; set; }

        [RepeaterColumn(RepeaterColumnType.Input, Header = "Nombre", Width = 600, Position = 4)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        [RequiredEx]
        public string Nombre { get; set; }

        [RepeaterColumn(RepeaterColumnType.Input, Header = "Código", Width = 200, Position = 5)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        [RequiredEx]
        public string Codigo { get; set; }
    }
}
