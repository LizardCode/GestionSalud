using LizardCode.Framework.Application.Common.Annotations;
using System;

namespace LizardCode.SalmaSalud.Application.Models.MonedasFechasCambio
{
    public class MonedasFechasCambioViewModel
    {
        public int IdMoneda { get; set; }

        [RequiredEx]
        public DateTime Fecha { get; set; }
        
        [RequiredEx]
        public double Cotizacion { get; set; }

    }
}
