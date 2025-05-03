using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class OrdenPagoRetencion
	{
        public int IdOrdenPago { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoRetencion { get; set; }
        public string NroRetencion { get; set; }
        public double BaseImponible { get; set; }
        public double Importe { get; set; }

    }
}
