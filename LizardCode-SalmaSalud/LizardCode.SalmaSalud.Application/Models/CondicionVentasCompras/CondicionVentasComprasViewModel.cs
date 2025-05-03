using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.CondicionVentasCompras
{
    public class CondicionVentasComprasViewModel
    {
        public int IdCondicion { get; set; }

        [RequiredEx]
        [StringLengthEx(45)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string Descripcion { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Number)]
        public int Dias { get; set; }

        [RequiredEx]
        public int IdTipoCondicion { get; set; }

        public SelectList MaestroTipoCondicion { get; set; }

    }
}
