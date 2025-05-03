using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.TiposAsientos
{
    public class TiposAsientosViewModel
    {
        public int IdTipoAsiento { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        public List<TiposAsientosCuentas> Items { get; set; }
        public SelectList MaestroCuentasContables { get; set; }
    }
}