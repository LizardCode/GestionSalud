using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.CargosBanco
{
    public class CargosBancoViewModel
    {
        public int IdCargoBanco { get; set; }

        [RequiredEx]
        public int IdEjercicio { get; set; }

        [RequiredEx]
        public int IdBanco { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }

        [RequiredEx]
        public DateTime? FechaReal { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        [RequiredEx]
        [StringLengthEx(100)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string Descripcion { get; set; }

        public List<CargosBancoItems> Items { get; set; }


        public SelectList MaestroBancos { get; set; }

        public SelectList MaestroEjercicios { get; set; }

        public SelectList MaestroCuentasContables { get; set; }

        public SelectList MaestroAlicuotas { get; set; }

    }
}
