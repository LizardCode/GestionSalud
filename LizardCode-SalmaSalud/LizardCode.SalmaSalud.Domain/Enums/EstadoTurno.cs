using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Enums
{    
    public enum EstadoTurno
    {
        [Description("Agendado")]
        Agendado = 1, 
        [Description("Confirmado")]
        Confirmado,
        [Description("Ausente con Aviso")]
        AusenteConAviso,
        [Description("Ausente sin Aviso")]
        AusenteSinAviso,
        [Description("Recepcionado")]
        Recepcionado,
        [Description("Atendido")]
        Atendido,
        [Description("Cancelado")]
        Cancelado,
        [Description("Re-Agendado")]
        ReAgendado,
        [Description("En Consultorio")]
        EnConsultorio
    }
}
