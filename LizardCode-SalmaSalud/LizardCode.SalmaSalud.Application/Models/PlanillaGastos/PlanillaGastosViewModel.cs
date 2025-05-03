using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.PlanillaGastos
{
    public class PlanillaGastosViewModel
    {
        public int IdPlanillaGastos { get; set; }


        [RequiredEx]
        public int IdEjercicio { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }

        #region Filtros

        public DateTime FiltroFechaDesde { get; set; }
        public DateTime FiltroFechaHasta { get; set; }
        public string FiltroAnnoMes { get; set; }
        public int FiltroNumero { get; set; }
        public string FiltroDescripcion { get; set; }
        public string FiltroIdPlanillaGastos { get; set; }
        public string FiltroIdEstadoPlanillaGastos { get; set; }
        public string FiltroTipoPlanillaGastos { get; set; }

        #endregion

        [RequiredEx]
        public int IdCuentaContable { get; set; }
        public string CuentaContable { get; set; }

        [RequiredEx]
        public string IdMoneda { get; set; }
        public string Moneda { get; set; }

        [RequiredEx]
        [StringLengthEx(80)]
        [AlphaNumericConstraint(AlphaNumConstraintType.Alpha)]
        public string Descripcion { get; set; }

        [RequiredEx]
        public double ImporteTotal { get; set; }
        
        public List<PlanillaGastosDetalle> Items { get; set; }

        public SelectList MaestroMonedas { get; set; }

        public SelectList MaestroEstadoPlanilla { get; set; }

        public SelectList MaestroEjercicios { get; set; }

        public SelectList MaestroComprobantes { get; set; }

        public SelectList MaestroAlicuotas { get; set; }

        public SelectList MaestroCuentasContables { get; set; }

		public SelectList MaestroCuentasPercepcion { get; set; }

		public IFormFile FileExcel { get; set; }
    }
}
