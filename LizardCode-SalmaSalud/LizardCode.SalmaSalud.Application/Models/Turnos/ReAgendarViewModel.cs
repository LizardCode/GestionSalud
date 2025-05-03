
namespace LizardCode.SalmaSalud.Application.Models.Turnos
{
    public class ReAgendarViewModel
    {
        public int IdTurno { get; set; }
        public string Especialidad { get; set; }
        public string Profesional { get; set; }
        public int IdProfesionalTurno { get; set; }

        public int IdEspecialidad { get; set; }
        public int IdProfesional { get; set; }
    }
}
