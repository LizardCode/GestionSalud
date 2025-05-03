using LizardCode.Framework.Helpers.Utilities;
using Mapster;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Globalization;

namespace LizardCode.SalmaSalud.Application.Models.PlanillaGastos
{
    public class PlanillaGastosDetalle : IRegister
    {
        public PlanillaGastosDetalle()
        {
            Fecha = DateTime.Now.ToString("dd/MM/yyyy");
        }

        [RepeaterColumn(Header = "", Position = 1, Hidden = true)]
        public int IdPlanillaGastos { get; set; }

        [RepeaterColumn(Header = "#", Position = 2, Width = 40, Readonly = true)]
        public int Item { get; set; }

        [RepeaterColumn(RepeaterColumnType.Input, Header = "C.U.I.T.", Width = 110, Position = 3)]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,8,1]", delimiters: "-", numericOnly: true)]
        public string CUIT { get; set; }

        [RequiredEx]
        [RepeaterColumn(RepeaterColumnType.Input, Header = "Proveedor", Width = 200, Position = 4)]
        public string Proveedor { get; set; }

        [RequiredEx]
        [RepeaterColumn(RepeaterColumnType.Input, Header = "Fecha", Width = 100, Position = 5)]
        [MaskConstraint(MaskConstraintType.Date, datePattern: "['d','m','Y']", delimiters: "/")]
        public string Fecha { get; set; }

        [RequiredEx]
        [RepeaterColumn(RepeaterColumnType.Select2, Header = "Tipo", Width = 120, Position = 6)]
        public int IdComprobante { get; set; }
        public string Comprobante { get; set; }

        [RequiredEx]
        [RepeaterColumn(RepeaterColumnType.Input, Header = "Número", Width = 120, Position = 7)]
        public string NumeroComprobante { get; set; }

		[RequiredEx]
		[RepeaterColumn(RepeaterColumnType.Input, Header = "Detalle", Width = 250, Position = 8)]
		[AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
		public string Detalle { get; set; }

		[RepeaterColumn(RepeaterColumnType.Input, Header = "CAE", Width = 140, Position = 9)]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[14]", numericOnly: true)]
        public string CAE { get; set; }

        [RepeaterColumn(RepeaterColumnType.Input, Header = "Vto. CAE", Width = 100, Position = 10)]
        [MaskConstraint(MaskConstraintType.Date, datePattern: "['d','m','Y']", delimiters: "/")]
        public string VencimientoCAE { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "No Gravado", Width = 150, Position = 11)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double? NoGravado { get; set; }

        [RequiredEx]
        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Subtotal", Width = 150, Position = 12)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Subtotal { get; set; }

        [RequiredEx]
        [RepeaterColumn(RepeaterColumnType.Select2, Header = "Alicuota", Width = 80, Position = 13)]
        public int IdAlicuota { get; set; }
        public double Alicuota { get; set; }

		[RepeaterColumn(RepeaterColumnType.Currency, Header = "Subtotal 2", Width = 150, Position = 14)]
		[AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
		public double? Subtotal2 { get; set; }

		[RepeaterColumn(RepeaterColumnType.Select2, Header = "Alicuota 2", Width = 80, Position = 15)]
		public int? IdAlicuota2 { get; set; }
		public double? Alicuota2 { get; set; }

		[RepeaterColumn(RepeaterColumnType.Select2, Header = "Cuenta Percepción", Width = 200, Position = 16)]
		public int? IdCuentaContablePercepcion { get; set; }
		public string CuentaContablePercepcion { get; set; }

		[RepeaterColumn(RepeaterColumnType.Currency, Header = "Imp. Percep.", Width = 130, Position = 17)]
		[AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
		public double? Percepcion { get; set; }

		[RepeaterColumn(RepeaterColumnType.Select2, Header = "Cuenta Percepción 2", Width = 200, Position = 18)]
		public int? IdCuentaContablePercepcion2 { get; set; }
		public string CuentaContablePercepcion2 { get; set; }

		[RepeaterColumn(RepeaterColumnType.Currency, Header = "Imp. Percep. 2", Width = 130, Position = 19)]
		[AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
		public double? Percepcion2 { get; set; }

        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Imp. Internos", Width = 130, Position = 20)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double? ImpuestosInternos { get; set; }

        [RequiredEx]
        [RepeaterColumn(RepeaterColumnType.Currency, Header = "Total", Width = 150, Position = 21, Readonly = true)]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Total { get; set; }

        public void Register(TypeAdapterConfig config)
        {
            string[] formats = { "dd/MM/yyyy", "MM/dd/yyyy" };

            config.NewConfig<PlanillaGastosDetalle, Domain.EntitiesCustom.PlanillaGastoItem>()
                .Map(d1 => d1.Fecha, s1 => DateTime.ParseExact(s1.Fecha, formats, CultureInfo.InvariantCulture, DateTimeStyles.None));

            config.NewConfig<PlanillaGastosDetalle, Domain.EntitiesCustom.PlanillaGastoItem>()
                .BeforeMapping((s, d) => {
                    if (s.VencimientoCAE.IsNull())
                        s.VencimientoCAE = string.Empty;
                })
                .Map(d2 => d2.VencimientoCAE, s2 => !string.IsNullOrEmpty(s2.VencimientoCAE) ? DateTime.ParseExact(s2.VencimientoCAE, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) : default);
                
        }
    }
}
