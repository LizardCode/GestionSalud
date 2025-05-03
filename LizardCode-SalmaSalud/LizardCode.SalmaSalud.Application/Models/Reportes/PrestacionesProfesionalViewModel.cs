using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.Reportes
{
    public class PrestacionesProfesionalViewModel
    {
        public int IdProfesional { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public SelectList MaestroProfesionales { get; set; }
    }
}