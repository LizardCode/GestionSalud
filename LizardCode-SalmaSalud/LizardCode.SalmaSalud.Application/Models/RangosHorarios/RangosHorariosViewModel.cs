using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.RangosHorarios
{
    public class RangosHorariosViewModel
    {
        public int IdRangoHorario { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        [RequiredEx]
        public int IdEspecialidad { get; set; }

        public string Especialidad { get; set; }

        public SelectList MaestroEspecialidades { get; set; }
    }
}