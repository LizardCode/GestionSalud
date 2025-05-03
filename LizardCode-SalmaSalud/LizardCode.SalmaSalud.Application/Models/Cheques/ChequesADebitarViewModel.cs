using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.Cheques
{
    public class ChequesADebitarViewModel
    {
        [RepeaterColumn(RepeaterColumnType.Check, Width = 100, Header = "Seleccionar", Position = 1)]
        public bool Seleccionar { get; set; }

        [RepeaterColumn(Header = "Id", Position = 2, Hidden = true)]
        public int IdCheque { get; set; }

        [RepeaterColumn(Header = "Nro. de Cheque", Width = 200, Position = 3, Readonly = true)]
        public string NroCheque { get; set; }

        [RepeaterColumn(Header = "Fecha", Width = 200, Position = 4, Readonly = true)]
        [MaskConstraint(MaskConstraintType.Date, datePattern: "['d','m','Y']", delimiters: "/")]
        public DateTime Fecha { get; set; }

        [RepeaterColumn(Header = "Fecha Dif.", Width = 200, Position = 5, Readonly = true)]
        [MaskConstraint(MaskConstraintType.Date, datePattern: "['d','m','Y']", delimiters: "/")]
        public string FechaDiferido { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Importe", Width = 0, Position = 6, Readonly = true)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "$", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }

    }
}
