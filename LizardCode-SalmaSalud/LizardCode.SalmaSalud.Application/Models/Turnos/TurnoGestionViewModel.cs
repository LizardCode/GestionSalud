using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace LizardCode.SalmaSalud.Application.Models.Turnos
{
    public class TurnoGestionViewModel
    {
        public TurnoGestionViewModel()
        {
            FechaDesde = DateTime.Now.Date;
            FechaHasta = DateTime.Now.AddDays(15).Date;
        }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int IdEspecialidad { get; set; }
        public int IdProfesional { get; set; }
        public int IdTipoTurno { get; set; }

        public SelectList MaestroEspecialidades { get; set; }
        public SelectList MaestroProfesionales { get; set; }
        public SelectList MaestroTiposTurno { get; set; }
    }
}