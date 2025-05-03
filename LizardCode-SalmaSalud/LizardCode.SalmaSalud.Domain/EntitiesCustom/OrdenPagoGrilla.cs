using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class OrdenPagoGrilla
    {
        public int IdOrdenPago { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public double Importe { get; set; }
    }
}
