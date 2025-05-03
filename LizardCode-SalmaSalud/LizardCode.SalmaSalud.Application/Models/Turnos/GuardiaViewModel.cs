using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace LizardCode.SalmaSalud.Application.Models.Turnos
{
    public class GuardiaViewModel
    {
        public int IdPaciente { get; set; }

        public string Observaciones { get; set; }


        [RequiredEx]
        [StringLengthEx(10)]
        public string Documento { get; set; }

        [RequiredEx]
        [StringLengthEx(50)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        public string Nombre { get; set; }

        [RequiredEx]
        public int IdTipoTelefono { get; set; }

        [StringLengthEx(50)]
        [RequiredEx]
        public string Telefono { get; set; }

        [EmailAddressEx]
        [StringLengthEx(100)]
        [RequiredEx]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "_.-@")]
        public string Email { get; set; }

        [RequiredEx]
        public int? IdFinanciador { get; set; }

        [RequiredEx]
        public int? IdFinanciadorPlan { get; set; }

        [RequiredEx]
        public string FinanciadorNro { get; set; }

        [RequiredEx]
        public DateTime? FechaNacimiento { get; set; }

        [RequiredEx]
        public string Nacionalidad { get; set; }

        public bool SinCobertura { get; set; }
        public SelectList MaestroFinanciadores { get; set; }

        public bool ForzarParticular { get; set; }
        public bool ForzarPadron { get; set; }
    }
}
