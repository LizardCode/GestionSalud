using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.FacturacionManual
{
    public class FacturacionManualViewModel
    {
        public int IdComprobanteVenta { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }
        public DateTime? Vto { get; set; }

        [RequiredEx]
        public int IdEjercicio { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        [RequiredEx]
        public int IdCliente { get; set; }

        [RequiredEx]
        public int IdSucursal { get; set; }
        public string Sucursal { get; set; }

        [RequiredEx]
        public int IdComprobante { get; set; }

        [RequiredEx]
        public string IdMonedaComprobante { get; set; }

        public string Moneda { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Cotizacion { get; set; }

        [StringLengthEx(45)]
        public string ReferenciaComercial { get; set; }

        public FacturacionManualDetalle Detalle { get; set; }

        public List<FacturacionManualDetalle> Items { get; set; }

        public bool AgentePercepcionAGIP { get; set; }
        public bool AgentePercepcionARBA { get; set; }

        public SelectList MaestroCuentas { get; set; }
        public SelectList MaestroClientes { get; set; }
        public SelectList MaestroAlicuotas { get; set; }
        public SelectList MaestroSucursales { get; set; }
        public SelectList MaestroMonedas { get; set; }
        public SelectList MaestroComprobantes { get; set; }
        public SelectList MaestroEjercicios { get; set; }
        public int IdCondicion { get; set; }
        public SelectList MaestroCondicion { get; set; }
    }
}
