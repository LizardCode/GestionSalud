using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;
using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.SdoCtaCteCli
{
    public class SdoCtaCteCliViewModel
    {
        public int IdSaldoCtaCteCli { get; set; }

        [RequiredEx]
        public DateTime FechaDesde { get; set; }

        [RequiredEx]
        public DateTime FechaHasta { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        public List<SdoCtaCteCliDetalle> Items { get; set; }

        public SelectList MaestroClientes { get; set; }
        public SelectList MaestroComprobantes { get; set; }
        public SelectList MaestroAlicuotas { get; set; }
        public IFormFile FileExcel { get; set; }

        public SdoCtaCteCliViewModel()
        {
            FechaDesde = DateTime.Now;
            FechaHasta = DateTime.Now;
        }
    }
}
