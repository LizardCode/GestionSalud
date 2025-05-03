using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.AperturaAutoCuentas
{
    public class AperturaAutoCuentasViewModel
    {
        public int IdAsientoApertura { get; set; }

        [RequiredEx]
        public int IdEjercicio { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }

        [RequiredEx]
        [StringLengthEx(80)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string Descripcion { get; set; }

        public List<AperturaAutoCuentasDetalle> Items { get; set; }

        public SelectList MaestroEjercicios { get; set; }

        public SelectList MaestroCuentasContables { get; set; }

    }
}
