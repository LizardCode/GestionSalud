using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.AnulaComprobantesVenta
{
    public class AnulaComprobantesVentaViewModel
    {
        public int IdComprobanteVenta { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }
        public DateTime? Vto { get; set; }

        [RequiredEx]
        public int IdEjercicio { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int IdCliente { get; set; }

        [RequiredEx]
        public int IdSucursalAnular { get; set; }
        [RequiredEx]
        public int IdComprobanteAnular { get; set; }
        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[8]", numericOnly: true)]
        public string NumeroComprobanteAnular { get; set; }

        public AnulaComprobantesVentaDetalle Detalle { get; set; }

        public List<AnulaComprobantesVentaDetalle> Items { get; set; }

        public SelectList MaestroClientes { get; set; }
        public SelectList MaestroAlicuotas { get; set; }
        public SelectList MaestroSucursales { get; set; }
        public SelectList MaestroComprobantes { get; set; }
        public SelectList MaestroEjercicios { get; set; }
        public int IdCondicion { get; set; }
        public SelectList MaestroCondicion { get; set; }
    }
}
