using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Turnos
{
    public class AsignarViewModel
    {
        public string Fecha { get; set; }
        public string Especialidad { get; set; }
        public string Profesional { get; set; }
        public int IdProfesionalTurno { get; set; }

        public string Observaciones { get; set; }

        public int IdPaciente { get; set; }


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

        public bool SinCobertura { get; set; }

        [RequiredEx]
        public DateTime? FechaNacimiento { get; set; }

        [RequiredEx]
        public string Nacionalidad { get; set; }

        public SelectList MaestroFinanciadores { get; set; }
    }
}
