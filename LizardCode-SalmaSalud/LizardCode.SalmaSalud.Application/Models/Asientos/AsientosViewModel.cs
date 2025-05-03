using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.Asientos
{
    public class AsientosViewModel
    {
        public int IdAsiento{ get; set; }

        [RequiredEx]
        public int IdEjercicio { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        [RequiredEx]
        public int IdTipoAsiento { get; set; }

        [RequiredEx]
        [StringLengthEx(80)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string Descripcion { get; set; }

        public List<AsientosDetalle> Items { get; set; }


        public SelectList MaestroTipoAsientos { get; set; }

        public SelectList MaestroEjercicios { get; set; }

        public SelectList MaestroCuentasContables { get; set; }

    }
}
