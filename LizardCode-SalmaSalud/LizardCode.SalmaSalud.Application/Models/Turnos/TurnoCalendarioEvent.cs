using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.FullCalendar
{
    [Serializable]
    public class TurnoCalendarioEvent
    {
        public string Title { get; set; }
        public bool AllDay { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }
        public string ClassNames { get; set; }
        public string Display { get; set; }

        public int Disponibles { get; set; } = 0;
    }
}
