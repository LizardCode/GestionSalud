using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.LiquidacionesProfesionales
{
    public class LiquidacionProfesionalViewModel
    {
        public LiquidacionProfesionalViewModel()
        {
            FechaFiltroDesde = DateTime.Now.AddDays(-60).Date;
            FechaFiltroHasta = DateTime.Now.Date;
        }

        public int IdLiquidacionProfesional { get; set; }

        [RequiredEx]
        public int IdInstitucion { get; set; }

        [RequiredEx]
        public int IdProfesional { get; set; }

        [RequiredEx]
        public DateTime FechaDesde { get; set; }

        [RequiredEx]
        public DateTime FechaHasta { get; set; }

        //[RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Redondeo { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double SubTotal { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Total { get; set; }

        //[RequiredEx]
        public string Observaciones { get; set; }


        public LiquidacionProfesionalPrestacionViewModel Detalle { get; set; }
        public List<LiquidacionProfesionalPrestacionViewModel> Prestaciones { get; set; }

        #region Filtros

        public DateTime FechaFiltroDesde { get; set; }
        public DateTime FechaFiltroHasta { get; set; }

        public int IdEstadoLiquidacionProfesional { get; set; }

        #endregion

        public SelectList MaestroInstituciones { get; set; }
        public SelectList MaestroProfesionales { get; set; }
        public SelectList MaestroEstados { get; set; }
    }
}
