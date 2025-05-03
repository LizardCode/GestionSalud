using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.CodigosRetencion
{
    public class CodigosRetencionViewModel
    {
        public int IdCodigoRetencion { get; set; }

        [RequiredEx]
        public int IdTipoRetencion { get; set; }

        [RequiredEx]
        [StringLengthEx(45)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string Descripcion { get; set; }

        [RequiredEx]
        public string Regimen { get; set; }

        #region Retención de Ganancias

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double ImporteNoSujetoGanancias { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double ImporteMinimoRetencionGanancias { get; set; }
        public List<CodigosRetencionDetalle> Items { get; set; }

        public bool AcumulaPagos { get; set; }

        #endregion

        #region Retención de Ing. Brutos

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double ImporteNoSujetoIngBrutos { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Percentage, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double PorcentajeRetencionIngBrutos { get; set; }

        public bool PadronRetencionAGIP { get; set; }

        public bool PadronRetencionARBA { get; set; }

        #endregion

        #region Retención de I.V.A.

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double ImporteNoSujetoIVA { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Percentage, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double PorcentajeRetencionIVA { get; set; }

        #endregion

        #region Retención de IVA Monotributo

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double ImporteNoSujetoIVAMonotributo { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Percentage, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double PorcentajeRetencionIVAMonotributo { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Numeric, decimalPlaces: 0)]
        public int CantidadMesesIVAMonotributo { get; set; }

        #endregion

        #region Retención de Ganancias Monotributo

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double ImporteNoSujetoGanMonotributo { get; set; }
        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Percentage, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double PorcentajeRetencionGanMonotributo { get; set; }
        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Numeric, decimalPlaces: 0)]
        public int CantidadMesesGanMonotributo { get; set; }

        #endregion

        #region Retención de S.U.S.S.

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double ImporteNoSujetoSUSS { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Percentage, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double PorcentajeRetencionSUSS { get; set; }

        #endregion

        public SelectList MaestroTipoRetencion { get; set; }

    }
}
