using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.CuentasContables
{
    public class CuentasContablesViewModel
    {
        public int IdCuentaContable { get; set; }

        [RequiredEx]
        [StringLengthEx(8)]
        public string CodigoCuenta { get; set; }

        [RequiredEx]
        [StringLengthEx(100)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string Descripcion { get; set; }

        [RequiredEx]
        public int IdRubroContable { get; set; }

        public int? IdCodigoObservacion { get; set; }

        public bool EsCtaGastos { get; set; }

        public SelectList MaestroCodigosObservacion { get; set; }
        public SelectList MaestroRubrosContables { get; set; }

    }
}
