using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;

namespace LizardCode.SalmaSalud.Application.Models.Articulos
{
    public class ArticulosViewModel
    {
        public int IdArticulo { get; set; }

        [RequiredEx]
        public int IdRubroArticulo { get; set; }

        public string CodigoBarras { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        [RequiredEx]
        public double Alicuota { get; set; }

        public string Detalle { get; set; }

        [RequiredEx]
        public int IdCuentaContableVentas { get; set; }

        [RequiredEx]
        public int IdCuentaContableCompras { get; set; }


        public SelectList MaestroRubros { get; set; }
        public SelectList MaestroAlicuotas { get; set; }
        public SelectList MaestroCuentasContablesVentas { get; set; }
        public SelectList MaestroCuentasContablesCompras { get; set; }
    }
}