using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class TurnosSolicitudDashboard
    {
        public string Profesional {  get; set; }
        public int AsignadosHoy { get; set; }
        public int AsignadosMes { get; set; }
        public int CanceladosMes { get; set; }
    }
}
