using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoTurnoSolicitud
    {
        [Description("Solicitado")]
        Solicitado = 1,
        [Description("Asignado")]
        Asignado,
        [Description("Cancelado")]
        Cancelado
    }
}