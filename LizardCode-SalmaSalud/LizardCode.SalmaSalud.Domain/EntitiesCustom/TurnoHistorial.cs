using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class TurnoHistorial
    {
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }

        public string FechaString { get; set; }
    }
}
