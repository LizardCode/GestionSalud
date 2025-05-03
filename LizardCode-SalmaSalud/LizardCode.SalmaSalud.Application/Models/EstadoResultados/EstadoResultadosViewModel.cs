using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace LizardCode.SalmaSalud.Application.Models.EstadoResultados
{
    public class EstadoResultadosViewModel
    {
        public int IdEjercicio { get; set; }
        public DateTime FechaHasta { get; set; }
        public SelectList MaestroEjercicios { get; set; }

    }
}