namespace LizardCode.SalmaSalud.Appointments.Domain.EntitiesCustom
{
    public class Turno : Entities.Turno
    {
        public string Especialidad { get; set; }
        public string Profesional { get; set; }
        public string Paciente { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Telefono { get; set; }
        public string Estado { get; set; }
        public string EstadoClase { get; set; }
        public bool FichaIncompleta { get; set; }
    }
}
