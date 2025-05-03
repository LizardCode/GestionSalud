namespace LizardCode.SalmaSalud.Appointments.Domain.EntitiesCustom
{
    public class TurnoProfesional
    {
        public int IdTurnoProfesional { get; set; }
        public int IdProfesional { get; set; }
        public string Profesional { get; set; }
        public DateTime PrimerTurnoDisponible { get; set; }
        public int CantidadTurnosDisponibles { get; set; }
    }
}
