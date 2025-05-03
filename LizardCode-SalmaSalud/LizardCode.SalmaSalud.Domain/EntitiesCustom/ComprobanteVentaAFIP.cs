using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ComprobanteVentaAFIP : Entities.ComprobanteVenta
    {
        public int DocTipo { get; set; }
        public int CbteTipo { get; set; }
        public string CUIT { get; set; }
        public int Concepto { get; set; }
        public bool EsCredito { get; set; }
        public bool EsMiPymes { get; set; }
        public DateTime FchServDesde { get; set; }
        public DateTime FchServHasta { get; set; }
        public DateTime? PeriodoAsocFchServDesde { get; set; }
        public DateTime? PeriodoAsocFchServHasta { get; set; }
        public List<ComprobanteVentaCbtAsociadoAFIP> ComprobanteVentaCbtAsociadosAFIP { get; set; }
        public List<ComprobanteVentaOpcionalAFIP> ComprobanteVentaOpcionalesAFIP { get; set; }
        public List<ComprobanteVentaItemAFIP> ComprobanteVentaItemsAFIP { get; set; }

    }
}
