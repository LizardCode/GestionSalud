using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class TurnosTotales
    {
        public int CanceladosHoy { get; set; }
        public int CanceladosMensual { get; set; }
        public int RecepcionadosHoy { get; set; }
        public int RecepcionadosMensual { get; set; }
        public int AtendidosHoy { get; set; }
        public int AtendidosMensual { get; set; }
        public int SobreTurnosHoy { get; set; }
        public int SobreTurnosMensual { get; set; }
        //public int AgendadosHoy { get; set; }
        public int AgendadosMensual { get; set; }

        public int AgendadosHoy { get; set; }
        public int TotalHoy { get; set; }

        public int ConfirmadosManiana { get; set; }
        public int AgendadosManiana { get; set; }
    }
}
