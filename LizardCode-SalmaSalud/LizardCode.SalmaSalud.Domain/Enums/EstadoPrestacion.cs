using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoPrestacion
    {
        [Description("Realizada")]
        Realizada = 1,
        [Description("Facturada Parcial")]
        FacturadaParcial,
        [Description("Facturada")]
        Facturada
    }
}
