using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoProfesionalTurno
    {
        [Description("Disponible")]
        Disponible = 1,
        [Description("Agendado")]
        Agendado
    }
}
