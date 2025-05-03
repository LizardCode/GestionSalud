using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Models.Turnos
{
    public class DetalleViewModel
    {
        public int Cantidad { get; set; }
        public string Fecha { get; set; }
        public string Especialidad { get; set; }
        public List<Custom.Turno> Turnos { get; set; }
    }
}
