using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.SdoCtaCtePrv
{
    public class SdoCtaCtePrvViewModel
    {
        public int IdSaldoCtaCtePrv { get; set; }

        [RequiredEx]
        public DateTime FechaDesde { get; set; }

        [RequiredEx]
        public DateTime FechaHasta { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        public List<SdoCtaCtePrvDetalle> Items { get; set; }

        public SelectList MaestroProveedores { get; set; }
        public SelectList MaestroComprobantes { get; set; }
        public SelectList MaestroAlicuotas { get; set; }
        public IFormFile FileExcel { get; set; }

        public SdoCtaCtePrvViewModel()
        {
            FechaDesde = DateTime.Now;
            FechaHasta = DateTime.Now;
        }
    }
}
