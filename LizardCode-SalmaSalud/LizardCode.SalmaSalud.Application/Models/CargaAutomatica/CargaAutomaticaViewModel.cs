using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.CargaAutomatica
{
    public class CargaAutomaticaViewModel
    {
        public int IdComprobanteCompra { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }
        [RequiredEx]
        public DateTime? FechaReal { get; set; }
        public DateTime? Vto { get; set; }

        [RequiredEx]
        public int IdEjercicio { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        [RequiredEx]
        public int IdProveedor { get; set; }

        [RequiredEx]
        public int IdComprobante { get; set; }

        public int? IdCentroCosto { get; set; }

        [RequiredEx]
        public string NumeroComprobante { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,4]", delimiters: "/", numericOnly: true)]
        public string AnnoMesDesde { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[2,4]", delimiters: "/", numericOnly: true)]
        public string AnnoMesHasta { get; set; }

        public int? IdMonedaEstimado { get; set; }

        [RequiredEx]
        public string IdMonedaComprobante { get; set; }

        public string Moneda { get; set; }

        [RequiredEx]
        [AutoNumericConstraint(AutoNumericConstraintType.Currency, currencySymbol: "", decimalCharacter: ",", digitGroupSeparator: ".")]
        public double Cotizacion { get; set; }

        public CargaAutomaticaDetalle Detalle { get; set; }

        public List<CargaAutomaticaDetalle> Items { get; set; }

        public CargaAutomaticaPercepciones Percepcion { get; set; }

        public List<CargaAutomaticaPercepciones> ListaPercepciones { get; set; }

        [MaskConstraint(MaskConstraintType.Custom, blocks: "[14]", numericOnly: true)]
        public string CAE { get; set; }

        public DateTime? VenciminetoCAE { get; set; }

        public SelectList MaestroCuentasPercepciones { get; set; }
        public SelectList MaestroProveedores { get; set; }
        public SelectList MaestroAlicuotas { get; set; }
        public SelectList MaestroMonedas { get; set; }
        public SelectList MaestroComprobantes { get; set; }
        public SelectList MaestroEjercicios { get; set; }
        public SelectList MaestroCentroCostos { get; set; }
        public int IdCondicion { get; set; }
        public SelectList MaestroCondicion { get; set; }

    }
}
