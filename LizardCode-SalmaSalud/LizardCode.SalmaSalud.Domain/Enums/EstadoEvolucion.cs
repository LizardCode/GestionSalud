using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoEvolucion
    {
        [Description("Realizada")]
        Realizada = 1,

        [Description("Fact. Parcial")]
        Facturada_Parcial,

        [Description("Facturada")]
        Facturada,

        [Description("Eliminada")]
        Eliminada
    }
}
