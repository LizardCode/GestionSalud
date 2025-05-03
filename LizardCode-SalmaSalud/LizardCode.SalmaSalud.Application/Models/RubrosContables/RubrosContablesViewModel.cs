using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.RubrosContables
{
    public class RubrosContablesViewModel
    {
        public int IdRubroContable { get; set; }

        [RequiredEx]
        [StringLengthEx(6)]
        public string CodigoRubro { get; set; }

        [RequiredEx]
        [StringLengthEx(40)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string Descripcion { get; set; }

        public int? IdRubroPadre { get; set; }

        public string RubroPadre { get; set; }

    }
}
