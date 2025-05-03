using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Monedas
{
    public class MonedasViewModel
    {
        public int IdMoneda { get; set; }

        [RequiredEx]
        [StringLengthEx(60)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string Descripcion { get; set; }
        
        [RequiredEx]
        [StringLengthEx(3)]
        public string Simbolo { get; set; }

        [RequiredEx]
        [StringLengthEx(3)]
        public string Codigo { get; set; }

    }
}
