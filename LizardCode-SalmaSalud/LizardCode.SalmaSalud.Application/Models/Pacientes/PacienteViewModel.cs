using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Pacientes
{
    public class PacienteViewModel
    {
        public int IdPaciente { get; set; }

        [RequiredEx]
        [StringLengthEx(50)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        public string Nombre { get; set; }

        [RequiredEx]
        public DateTime? FechaNacimiento { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[8]", numericOnly: true)]
        [Remote("ValidarNroDocumento", "Pacientes", AdditionalFields = "IdPaciente", ErrorMessage = "Ya existe un paciente para el documento ingresado.", HttpMethod = "POST")]
        public string Documento { get; set; }

        [RequiredEx]
        public int IdNacionalidad { get; set; }
        //public string Nacionalidad { get; set; }

        //[RequiredEx]
        public int? IdFinanciador { get; set; }

        //[RequiredEx]
        public int? IdFinanciadorPlan { get; set; }

        //[RequiredEx]
        [Remote("ValidarFinanciadorNro", "Pacientes", AdditionalFields = "IdPaciente", ErrorMessage = "Ya existe un paciente para el nro. de Afiliado ingresado.", HttpMethod = "POST")]
        public string FinanciadorNro { get; set; }

        public bool SinCobertura { get; set; }

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

        public bool Habilitado { get; set; }

        #region Filtros

        //public string FiltroCUIT { get; set; }
        public string FiltroNombre { get; set; }

        public string FiltroDocumento { get; set; }

        #endregion

        public SelectList MaestroTipoTelefono { get; set; }
        public SelectList MaestroFinanciadores { get; set; }
        public SelectList MaestroNacionalidades { get; set; }

        public DateTime? UltimaAtencion { get; set; }
    }
}
