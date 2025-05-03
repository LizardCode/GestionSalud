using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;

namespace LizardCode.SalmaSalud.Application.Models.OrdenesPago
{
    public class OrdenesPagoPlanillasGastos
    {
        [RepeaterColumn(RepeaterColumnType.Check, Width = 100, Header = "Seleccionar", Position = 1)]
        public bool Seleccionar { get; set; }

        [RepeaterColumn(Header = "Id", Position = 2, Width = 100, Readonly = true)]
        public int IdPlanillaGastos { get; set; }

        [RepeaterColumn(Header = "Fecha", Width = 100, Position = 3, Readonly = true)]
        [MaskConstraint(MaskConstraintType.Date, datePattern: "['d','m','Y']", delimiters: "/")]
        public DateTime Fecha { get; set; }

        [RepeaterColumn(Header = "Descripción", Position = 4, Readonly = true)]
        public string Descripcion { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Importe", Width = 130, Position = 5, Readonly = true)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "$", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }

    }
}
