using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizadCode.SalmaSalud.Notifications.Domain.Enums
{
    public enum EventosChatApi
    {
        [Description("Turno Asignado")]
        TurnoAsignado = 1,
        [Description("Turno Confirmado")]
        TurnoConfirmado = 2,
        [Description("Turno Re-Agendado")]
        TurnoReAgendado = 3,
        [Description("Turno Cancelado")]
        TurnoCancelado = 4,
        [Description("Sobre-Turno Asignado")]
        SobreTurnoAsignado = 5,
        [Description("Turno Recepcionado")]
        TurnoRecepcionado = 6,
        [Description("Recordatorio Turno")]
        TurnoRecordatorio = 7
    }
}
