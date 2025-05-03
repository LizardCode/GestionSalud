using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Enums
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
        TurnoRecordatorio = 7,
        [Description("Demanda Espontánea")]
        DemandaEspontanea = 8,
        [Description("Código Acceso")]
        CodigoAcceso = 9,
        [Description("Bienvenida")]
        Bienvenida = 10,
        [Description("Guardia")]
        Guardia = 11,
        [Description("Guardia Llamado")]
        GuardiaLlamado = 12,
        [Description("Guardia Cancelado")]
        GuardiaCancelado = 13
    }
}
