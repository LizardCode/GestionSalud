using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class PrestacionFinanciador
    {
        public int IdEvolucionPrestacion { get; set; }
        public int IdFinanciador { get; set; }
        public int IdEmpresa { get; set; }
        public int IdTipoPrestacion { get; set; }
        public DateTime Fecha { get; set; }

        public string TipoPrestacion { get; set; }
        public string Financiador { get; set; }
        public string Especialidad { get; set; }
        public string Prestacion { get; set; }
        public string Codigo { get; set; }

        public double Valor { get; set; }
    }
}
