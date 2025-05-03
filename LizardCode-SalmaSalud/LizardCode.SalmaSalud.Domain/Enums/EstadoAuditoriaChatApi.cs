using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoAuditoriaChatApi
    {
        [Description("Enviado")]
        Enviado = 1,

        [Description("Error")]
        Error,

        [Description("Pendiente")]
        Pendiente,
    }
}
