using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.DepositosBanco
{
    public class DepositosBancoViewModel
    {
        public int IdDepositoBanco { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }
        
        [RequiredEx]
        public int IdEjercicio { get; set; }

        [RequiredEx]
        public int IdBanco { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }

        [RequiredEx]
        public string IdMoneda { get; set; }

        public string Moneda { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double? Cotizacion { get; set; }

        #region Detalle Deposito Banco

        public DepositosBancoDetalle Detalle { get; set; }

        public List<DepositosBancoDetalle> Items { get; set; }

        #endregion

        #region Filtros

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int? NumeroDepositoBanco { get; set; }

        #endregion

        #region Lookups

        public SelectList MaestroMonedas { get; set; }

        public SelectList MaestroBancos { get; set; }

        public SelectList MaestroEjercicios { get; set; }

        public SelectList MaestroTipoDepositoBanco { get; set; }

        #endregion

    }
}