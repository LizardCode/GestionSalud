using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ChequeTerceros
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Banco { get; set; }
        public string NroCheque { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVto { get; set; }
        public double Importe { get; set; }
    }
}
