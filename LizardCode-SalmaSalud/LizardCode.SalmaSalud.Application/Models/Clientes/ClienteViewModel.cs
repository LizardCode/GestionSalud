using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.Clientes
{
    public class ClienteViewModel
    {
        public int IdCliente { get; set; }

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
        public int IdTipoDocumento { get; set; }

        //[RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[8]", numericOnly: true)]
        [Remote("ValidarNroDocumento", "Clientes", AdditionalFields = "IdCliente", ErrorMessage = "Ya existe un cliente para el documento ingresado", HttpMethod = "POST")]
        public string Documento { get; set; }

        //[RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,8,1]", delimiters: "-", numericOnly: true)]
        [Remote("ValidarNroCUIT", "Clientes", AdditionalFields = "IdCliente", ErrorMessage = "Error en el Nro de CUIT Ingresado", HttpMethod = "POST")]
        public string CUIT { get; set; }

        [MaskConstraint(MaskConstraintType.Custom, blocks: "[11]", numericOnly: true)]
        public string NroIBr { get; set; }

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

        #region Filtros

        public string FiltroCUIT { get; set; }
        public string FiltroRazonSocial { get; set; }

        #endregion

        public SelectList MaestroTipoIVA { get; set; }
        public SelectList MaestroTipoDocumento { get; set; }
        public SelectList MaestroTipoTelefono { get; set; }

    }
}
