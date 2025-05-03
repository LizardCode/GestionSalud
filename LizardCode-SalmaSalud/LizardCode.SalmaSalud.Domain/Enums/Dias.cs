using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum Dias
    {
        [Description("LUNES")]
        Lunes = 1,
        [Description("MARTES")]
        Martes = 2,
        [Description("MIÉRCOLES")]
        Miercoles = 3,
        [Description("JUEVES")]
        Jueves = 4,
        [Description("VIERNES")]
        Viernes = 5,
        [Description("SÁBADO")]
        Sabado = 6
    }
}
