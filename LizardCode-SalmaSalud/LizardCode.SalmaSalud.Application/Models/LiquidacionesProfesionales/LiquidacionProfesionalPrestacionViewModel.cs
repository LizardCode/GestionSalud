using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.LiquidacionesProfesionales
{
    public class LiquidacionProfesionalPrestacionViewModel
    {
        //[RepeaterColumn(Header = "Prestación", ControlType = RepeaterColumnType.Hidden, Width = 0, Position = 1)]
        //[RequiredEx]
        //public int IdPrestacion { get; set; }

        //[RepeaterColumn(Header = "Otra Prestación", ControlType = RepeaterColumnType.Hidden, Width = 0, Position = 2)]
        //[RequiredEx]
        //public int IdOtraPrestacion { get; set; }

        //public string Descripcion { get; set; } //Fecha - Codigo - Nombre

        //[RepeaterColumn(RepeaterColumnType.Currency, Header = "Valor", Width = 75, Position = 5, Readonly = true)]
        //[AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".", decimalPlaces: 0)]
        //public double Valor { get; set; }

        //[RepeaterColumn(RepeaterColumnType.Currency, Header = "Fijo", Width = 75, Position = 5, Readonly = true)]
        //[AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".", decimalPlaces: 0)]
        //public double ValorFijo { get; set; }

        //[RepeaterColumn(RepeaterColumnType.Currency, Header = "Porcentaje", Width = 75, Position = 5, Readonly = true)]
        //[AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".", decimalPlaces: 0)]
        //public double Porcentaje { get; set; }

        //[RepeaterColumn(RepeaterColumnType.Currency, Header = "Total", Width = 75, Position = 6)]
        //[AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".", decimalPlaces: 0)]
        //public double Total { get; set; }

        [MasterDetailColumn(Header = "", Position = 1, Hidden = true)]
        public int IdLiquidacionProfesional { get; set; }

        [MasterDetailColumn(Header = "", Position = 2, Hidden = true)]
        public int? IdEvolucionPrestacion { get; set; }

        [MasterDetailColumn(Header = "", Position = 3, Hidden = true)]
        public int? IdEvolucionOtraPrestacion { get; set; }

        //[RequiredEx]
        [MasterDetailColumn(Header = "Descripción", Width = 0, Position = 4)]
        public string Descripcion { get; set; }

        [MasterDetailColumn(Header = "Financiador", Width = 250, Position = 5)]
        public string Financiador { get; set; }

        [MasterDetailColumn(Header = "Valor", Width = 110, Position = 6)]
        //[RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        public double Valor { get; set; }

        [MasterDetailColumn(Header = "Valor Fijo", Width = 110, Position = 7)]
        //[RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        public double Fijo { get; set; }

        [MasterDetailColumn(Header = "Porc.", Width = 110, Position = 8)]
        //[RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.None)]
        public double Porcentaje { get; set; }

        [MasterDetailColumn(Header = "Valor Porc.", Width = 110, Position = 9)]
        //[RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        public double ValorPorcentaje { get; set; }

        [MasterDetailColumn(Header = "Total", Width = 110, Position = 10)]
        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        [MasterDetailFormat(Format = MasterDetailColumnFormat.Currency)]
        public double Total { get; set; }

        [MasterDetailColumn(Header = "", Position = 11, Hidden = true)]
        public int? IdGuardia { get; set; }

    }
}
