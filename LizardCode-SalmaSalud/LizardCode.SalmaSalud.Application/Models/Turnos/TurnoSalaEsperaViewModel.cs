using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.SalmaSalud.Application.Models.Evoluciones;
using System;

namespace LizardCode.SalmaSalud.Application.Models.Turnos
{
    public class TurnoSalaEsperaViewModel
    {
        public int IdEspecialidad { get; set; }
        public int IdProfesional { get; set; }

        public EvolucionRecetaViewModel Receta { get; set; }
        public EvolucionOrdenViewModel Orden { get; set; }
        public SelectList MaestroEspecialidades { get; set; }
        public SelectList MaestroProfesionales { get; set; }
        public SelectList MaestroVademecum { get; set; }
    }
}
