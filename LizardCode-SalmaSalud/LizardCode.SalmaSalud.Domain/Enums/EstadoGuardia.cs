﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Enums
{
    public enum EstadoGuardia
    {
        [Description("Pendiente")]
        Pendiente = 1,

        [Description("Liquidada")]
        Liquidada
    }
}
