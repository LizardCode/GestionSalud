namespace LizadCode.SalmaSalud.Notifications.Domain.EntitiesCustom
{
    public class TurnoAConfirmar
    {
        public int IdTurno { get; set; }
        public string Profesional { get; set; }
        public string Paciente { get; set; }
        public string Hora { get; set; }
        public string Telefono { get; set; }
    }
}
