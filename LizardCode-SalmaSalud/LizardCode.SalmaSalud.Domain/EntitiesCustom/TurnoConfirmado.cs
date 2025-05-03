using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class TurnoConfirmado
    {
        public int IdTurno { get; set; }
        public string Profesional { get; set; }
        public string Paciente { get; set; }
        public string Hora { get; set; }
        public string Telefono { get; set; }
    }
}
