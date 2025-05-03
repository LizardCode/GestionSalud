using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.Laboratorios
{
    public class LaboratorioViewModel
    {
        public int IdProveedor { get; set; }

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
        public List<int?> Empresas { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,8,1]", delimiters: "-", numericOnly: true)]
        [Remote("ValidarNroCUIT", "Proveedores", AdditionalFields = "IdProveedor", ErrorMessage = "Error en el Nro de CUIT Ingresado", HttpMethod = "POST")]
        public string CUIT { get; set; }

        [MaskConstraint(MaskConstraintType.Custom, blocks: "[11]", numericOnly: true)]
        public string NroIBr { get; set; }

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

        public int? IdCodigoRetencionGanancias { get; set; }
        public int? IdCodigoRetencionIVA { get; set; }
        public int? IdCodigoRetencionIBr { get; set; }
        public int? IdCodigoRetencionSUSS { get; set; }
        public int? IdCodigoRetencionGananciasMonotributo { get; set; }
        public int? IdCodigoRetencionIVAMonotributo { get; set; }

        public List<LaboratorioServicioViewModel> Servicios { get; set; }

        #region Filtros

        public string FiltroCUIT { get; set; }
        public string FiltroRazonSocial { get; set; }

        #endregion

        public SelectList MaestroTipoIVA { get; set; }
        public SelectList MaestroTipoTelefono { get; set; }
        public SelectList MaestroEmpresas { get; set; }

        public SelectList MaestroRetencionGanancias { get; set; }
        public SelectList MaestroRetencionIVA { get; set; }
        public SelectList MaestroRetencionIBr { get; set; }
        public SelectList MaestroRetencionSUSS { get; set; }
        public SelectList MaestroRetencionGananciasMonotributo { get; set; }
        public SelectList MaestroRetencionIVAMonotributo { get; set; }
    }
}