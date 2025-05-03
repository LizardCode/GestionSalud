using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Application.Models.Asientos;
using Microsoft.AspNetCore.Http;

namespace LizardCode.SalmaSalud.Application.Models.Financiadores
{
    public class FinanciadorViewModel
    {
        public int IdFinanciador { get; set; }

        //[RequiredEx]
        //[StringLengthEx(50)]
        //[AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        //public string RazonSocial { get; set; }

        [RequiredEx]
        [StringLengthEx(50)]
        [AlphaNumericConstraint(AlphaNumConstraintType.AlphaPlusCharset, charset: "'")]
        public string Nombre { get; set; }

        [RequiredEx]
        public string NroFinanciador { get; set; }

        public bool Capita { get; set; }

        [RequiredEx]
        public int IdTipoIVA { get; set; }

        public string TipoIVA { get; set; }

        [RequiredEx]
        public List<int?> Empresas { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,8,1]", delimiters: "-", numericOnly: true)]
        [Remote("ValidarNroCUIT", "Financiadores", AdditionalFields = "IdFinanciador", ErrorMessage = "Error en el Nro de CUIT Ingresado", HttpMethod = "POST")]
        public string CUIT { get; set; }

        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
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
        public List<FinanciadorPlanViewModel> Items { get; set; }
        //public List<FinanciadorPrestacionViewModel> Prestaciones { get; set; }

        #region Filtros

        //public string FiltroCUIT { get; set; }
        public string FiltroNombre { get; set; }

        #endregion

        public SelectList MaestroTipoIVA { get; set; }
        public SelectList MaestroTipoTelefono { get; set; }
        public IFormFile FileExcel { get; set; }
    }
}
