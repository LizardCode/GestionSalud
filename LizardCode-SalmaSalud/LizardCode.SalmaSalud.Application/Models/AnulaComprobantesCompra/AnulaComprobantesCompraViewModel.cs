using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.AnulaComprobantesCompra
{
    public class AnulaComprobantesCompraViewModel
    {
        public int IdComprobanteCompra { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }
        [RequiredEx]
        public DateTime? Vto { get; set; }

        [RequiredEx]
        public int IdEjercicio { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int IdProveedor { get; set; }

        public string NumeroComprobante { get; set; }

        [RequiredEx]
        public int IdComprobanteAnular { get; set; }
        [RequiredEx]
        public string NumeroComprobanteAnular { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.String, blocks: "[14]", numericOnly: true)]
        public string CAE { get; set; }

        [RequiredEx]
        public DateTime VenciminetoCAE { get; set; }


        public AnulaComprobantesCompraDetalle Detalle { get; set; }

        public List<AnulaComprobantesCompraDetalle> Items { get; set; }

        public SelectList MaestroProveedores { get; set; }
        public SelectList MaestroAlicuotas { get; set; }
        public SelectList MaestroComprobantes { get; set; }
        public SelectList MaestroEjercicios { get; set; }
        public int IdCondicion { get; set; }
        public SelectList MaestroCondicion { get; set; }

    }
}
