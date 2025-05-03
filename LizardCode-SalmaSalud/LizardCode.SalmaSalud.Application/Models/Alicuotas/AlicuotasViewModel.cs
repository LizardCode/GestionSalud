using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Alicuotas
{
    public class AlicuotasViewModel
    {
        public int IdAlicuota { get; set; }

        [RequiredEx]
        public int IdTipoAlicuota { get; set; }

        [RequiredEx]
        [StringLengthEx(45)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string Descripcion { get; set; }

        [RequiredEx]
        public double Valor { get; set; }

        public int? CodigoAFIP { get; set; }

        public SelectList MaestroTipoAlicuotas { get; set; }

    }
}
