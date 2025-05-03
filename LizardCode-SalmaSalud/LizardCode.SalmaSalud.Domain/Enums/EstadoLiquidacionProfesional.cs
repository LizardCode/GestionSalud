using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoLiquidacionProfesional
    {
        [Description("Pendiente")]
        Pendiente = 1,
        [Description("Facturada")]
        Facturada
    }
}
