using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.PortalPacientes
{
    public class MisDatosViewModel
    {
        [StringLengthEx(50)]
        [RequiredEx]
        public string Telefono { get; set; }

        [EmailAddressEx]
        [StringLengthEx(100)]
        [RequiredEx]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "_.-@")]
        public string Email { get; set; }

        [RequiredEx]
        public DateTime? FechaNacimiento { get; set; }
    }
}
