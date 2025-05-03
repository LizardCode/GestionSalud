using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.ProveedoresContactos
{
    public class ProveedorContactoViewModel
    {
        public int IdProveedorContacto { get; set; }

        [RequiredEx]
        public int IdProveedor { get; set; }

        [RequiredEx]
        [StringLengthEx(50)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string Nombre { get; set; }

        [RequiredEx]
        [StringLengthEx(50)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        public string Apellido { get; set; }

        public string Cargo { get; set; }

        [RequiredEx]
        public int IdTipoTelefono { get; set; }

        [RequiredEx]
        [StringLengthEx(50)]
        public string Telefono { get; set; }

        [RequiredEx]
        [EmailAddressEx]
        [StringLengthEx(100)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "_.-@")]
        public string Email { get; set; }

        public SelectList MaestroProveedores { get; set; }
        public SelectList MaestroTipoTelefono { get; set; }

    }
}
