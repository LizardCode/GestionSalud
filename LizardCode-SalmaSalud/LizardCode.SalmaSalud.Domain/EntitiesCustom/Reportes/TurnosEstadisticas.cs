using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class TurnosEstadisticasAusentes
    {
        public int ConAviso { get; set; }
        public int SinAviso { get; set; }
        public int Cancelados { get; set; }
    }

    public class TurnosEstadisticasEstados
    {
        public string Estado { get; set; }
        public int Cantidad { get; set; }
    }
}
