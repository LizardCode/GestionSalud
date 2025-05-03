using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class Evolucion : Entities.Evolucion
    {        
        public string Especialidad { get; set; }
        public string Profesional { get; set; }
        public string Paciente { get; set; }
        public string Estado { get; set; }
        public string EstadoClase { get; set; }
        public string TipoTurno { get; set; }
        public string TipoTurnoDescripcion { get; set; }
        public DateTime FechaTurno { get; set; }
        public string Financiador { get; set; }
        public string FinanciadorPlan { get; set; }
        public string FinanciadorNro { get; set; }
        public string Empresa { get; set; }
    }
}
