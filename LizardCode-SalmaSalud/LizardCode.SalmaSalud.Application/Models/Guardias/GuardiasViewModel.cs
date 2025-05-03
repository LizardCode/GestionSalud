using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.SalmaSalud.Application.Models.LiquidacionesProfesionales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Guardias
{
    public class GuardiasViewModel
    {
        public GuardiasViewModel()
        {
            FechaFiltroDesde = DateTime.Now.AddDays(-60).Date;
            FechaFiltroHasta = DateTime.Now.Date;
        }

        public int IdGuardia { get; set; }

        [RequiredEx]
        public int IdProfesional { get; set; }

        [RequiredEx]
        public DateTime Desde { get; set; }

        [RequiredEx]
        public DateTime Hasta { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Total { get; set; }

        //[RequiredEx]
        public string Observaciones { get; set; }

        #region Filtros

        public DateTime FechaFiltroDesde { get; set; }
        public DateTime FechaFiltroHasta { get; set; }

        public int IdEstadoGuardia { get; set; }

        #endregion

        public SelectList MaestroProfesionales { get; set; }
        public SelectList MaestroEstados { get; set; }
    }
}
