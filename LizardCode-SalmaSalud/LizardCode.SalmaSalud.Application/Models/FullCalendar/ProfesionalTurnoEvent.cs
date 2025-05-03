using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.FullCalendar
{
    [Serializable]
    public class ProfesionalTurnoEvent
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
        public int IdProfesionalTurno { get; set; }

        public string Fecha { get; set; }

        public bool Clickable { get; set; } = true;

        public string Paciente { get; set; }

        public string Descripcion { get; set; }

        //public string BadgeClass { get; set; }

        public string GroupId { get; set; }
        public string Display { get; set; }
    }
}
