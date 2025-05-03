using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.SalmaSalud.Application.Models.Sucursales
{
    public class SucursalesViewModel
    {
        public int IdSucursal { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        [RequiredEx]
        [StringLengthEx(5)]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[5]", numericOnly: true)]
        public string CodigoSucursal { get; set; }

        [RequiredEx]
        public string Exenta { get; set; }

        [RequiredEx]
        public string Webservice { get; set; }

        public SelectList MaestroCommonBool { get; set; }

    }
}