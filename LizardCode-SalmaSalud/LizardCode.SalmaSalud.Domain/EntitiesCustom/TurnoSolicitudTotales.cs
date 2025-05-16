using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class TurnoSolicitudTotales
    {
        public int Total { get; set; }
        public int Solicitados { get; set; }
        public int Asignados { get; set; }
        public int Cancelados { get; set; }
    }
}
