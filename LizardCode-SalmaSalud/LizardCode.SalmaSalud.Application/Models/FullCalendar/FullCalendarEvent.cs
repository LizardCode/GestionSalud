using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.FullCalendar
{
    [Serializable]
    public class FullCalendarEvent
    {
        public string Title { get; set; }
        public bool AllDay { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }
        public string ClassNames { get; set; }
        public bool DisplayEventEnd { get; set; }

        //Extended Props
        public int IdProfesional { get; set; }
        public string ShortTitle { get; set; }
        public string LongTitle { get; set; }

        //public string BadgeClass { get; set; }

        public string GroupId { get; set; }

        public string Display { get; set; }
    }
}
