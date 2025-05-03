using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using Microsoft.AspNetCore.Http;

namespace LizardCode.SalmaSalud.Application.Models.Empresas
{
    public class EmpresaViewModel
    {
        public int IdEmpresa { get; set; }

        [RequiredEx]
        [StringLengthEx(50)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string RazonSocial { get; set; }

        [RequiredEx]
        [StringLengthEx(50)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        public string NombreFantasia { get; set; }

        [RequiredEx]
        public int IdTipoIVA { get; set; }

        public string TipoIVA { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,8,1]", delimiters: "-", numericOnly: true)]
        public string CUIT { get; set; }

        [MaskConstraint(MaskConstraintType.Custom, blocks: "[11]", numericOnly: true)]
        public string NroIBr { get; set; }

        [RequiredEx]
        public int IdTipoTelefono { get; set; }

        [StringLengthEx(50)]
        public string Telefono { get; set; }

        [EmailAddressEx]
        [StringLengthEx(100)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "_.-@")]
        public string Email { get; set; }

        public bool AgentePercepcionAGIP { get; set; }

        public bool AgentePercepcionARBA { get; set; }

        [RequiredEx]
        [StringLengthEx(200)]
        public string Direccion { get; set; }

        [StringLengthEx(15)]
        public string CodigoPostal { get; set; }

        [StringLengthEx(2)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaNumeric)]
        public string Piso { get; set; }

        [StringLengthEx(4)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaNumeric)]
        public string Departamento { get; set; }

        [StringLengthEx(50)]
        public string Localidad { get; set; }

        [StringLengthEx(50)]
        public string Provincia { get; set; }

        [RequiredEx]
        public double Latitud { get; set; }

        [RequiredEx]
        public double Longitud { get; set; }


        public SelectList MaestroTipoIVA { get; set; }
        public SelectList MaestroTipoTelefono { get; set; }
        public SelectList MaestroEmpresas { get; set; }

        public int IdEmpresaCopiar { get; set; }

        public bool EnableProdAFIP { get; set; }

        [RequiredEx]
        public DateTime FechaInicioActividades { get; set; }

        [StringLengthEx(2)]
        public string TurnosHoraInicio { get; set; }

        [StringLengthEx(2)]
        public string TurnosMinutosInicio { get; set; }

        [StringLengthEx(2)]
        public string TurnosHoraFin { get; set; }

        [StringLengthEx(2)]
        public string TurnosMinutosFin { get; set; }

        [StringLengthEx(50)]
        public string TurnosIntervalo { get; set; }
        public IFormFile RecetaLogo { get; set; }
        public string UploadedTipoRecetaLogo { get; set; }
        public string UploadedRecetaLogo { get; set; }
        public bool RemovedRecetaLogo { get; set; }
    }
}
