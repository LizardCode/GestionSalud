using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.Recibos
{
    public class RecibosViewModel
    {
        public int IdRecibo { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }

        [RequiredEx]
        public int IdEjercicio { get; set; }

        [RequiredEx]
        public int IdTipoRecibo { get; set; }

        [RequiredEx]
        public int IdCliente { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Total { get; set; }

        [RequiredEx]
        public string IdMoneda { get; set; }

        public string Moneda { get; set; }

        [RequiredEx]
        public string IdMonedaCobro { get; set; }

        public string MonedaCobro { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Cotizacion { get; set; }

        #region Detalle Cobro

        public RecibosDetalle Detalle { get; set; }

        public List<RecibosDetalle> Items { get; set; }

        #endregion

        #region Detalle Anticipos

        public RecibosAnticipo Anticipo { get; set; }

        public List<RecibosAnticipo> Anticipos { get; set; }

        #endregion

        #region Detalle Retencion

        public RecibosRetencion Retencion { get; set; }

        public List<RecibosRetencion> Retenciones { get; set; }

        #endregion

        #region Detalle Imputaciones

        public List<RecibosImputacion> Imputaciones { get; set; }

        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double? Redondeo { get; set; }

        #endregion


        #region Filtros

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int IdEstadoRecibo { get; set; }
        public int? NumeroRecibo { get; set; }

        #endregion

        #region Lookups

        public SelectList MaestroMonedas { get; set; }

        public SelectList MaestroClientes { get; set; }

        public SelectList MaestroTipoRecibo { get; set; }

        public SelectList MaestroBancos { get; set; }

        public SelectList MaestroCategoriasRetencion { get; set; }

        public SelectList MaestroCuentasContables { get; set; }

        public SelectList MaestroEstadoRecibos { get; set; }

        public SelectList MaestroEjercicios { get; set; }

        public SelectList MaestroTipoCobro { get; set; }

        #endregion

    }
}