using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.OrdenesPago
{
    public class OrdenesPagoViewModel
    {
        public int IdOrdenPago { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }

        [RequiredEx]
        public int IdEjercicio { get; set; }

        [RequiredEx]
        public int IdTipoOrdenPago { get; set; }

        public int? IdProveedor { get; set; }

        public int? IdCuentaContable { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Importe { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double ImporteAnticipo { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double ImporteVarios { get; set; }

        [RequiredEx]
        public string IdMoneda { get; set; }

        public string Moneda { get; set; }

        [RequiredEx]
        public string IdMonedaPago { get; set; }

        public string MonedaPago { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Cotizacion { get; set; }

        #region Detalle Imputaciones o Planilla Gastos

        public List<OrdenesPagoImputacion> Imputaciones { get; set; }

        public List<OrdenesPagoPlanillasGastos> PlanillasGastos { get; set; }

        #endregion

        #region Detalle Anticipos

        public OrdenesPagoAnticipo Anticipo { get; set; }

        public List<OrdenesPagoAnticipo> Anticipos { get; set; }

        #endregion

        #region Detalle Pago

        public OrdenesPagoDetalle Detalle { get; set; }

        public List<OrdenesPagoDetalle> Items { get; set; }

        #endregion

        #region Retenciones

        public List<OrdenesPagoRetencion> Retenciones { get; set; }

        #endregion

        #region Filtros

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int IdEstadoOrdenPago { get; set; }
        public int IdOrdenPagoTipo { get; set; }
        public int NumeroOrdenPago { get; set; }

        #endregion

        #region Lookups

        public SelectList MaestroMonedas { get; set; }

        public SelectList MaestroProveedores { get; set; }

        public SelectList MaestroTipoOrdenPago { get; set; }

        public SelectList MaestroBancos { get; set; }

        public SelectList MaestroEstadoOrdenPago { get; set; }

        public SelectList MaestroEjercicios { get; set; }

        public SelectList MaestroTipoPago { get; set; }

        public SelectList MaestroChequesTerceros { get; set; }

        public SelectList MaestroCuentas { get; set; }

        public SelectList MaestroCuentasGastos { get; set; }

        #endregion

    }
}