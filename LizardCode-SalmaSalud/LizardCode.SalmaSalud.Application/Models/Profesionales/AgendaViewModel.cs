using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Profesionales
{
    public class AgendaViewModel
    {
        public int IdProfesional { get; set; }
        public string Profesional { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string IntervaloTurnos { get; set; }
    }
}