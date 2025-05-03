using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class EvolucionesEstadisticas
    {
        public int Turnos {  get; set; }
        public int SobreTurnos { get; set; }
        public int DemandaEspontanea { get; set; }
        public int Guardia { get; set; }
    }

    public class EvolucionesEstadisticasFinanciador
    {
        public string Financiador { get; set; }
        public int Cantidad { get; set; }
    }

    public class EvolucionesEstadisticasEspecialidad
    {
        public string Especialidad { get; set; }
        public int Cantidad { get; set; }
    }
}
