namespace LizardCode.SalmaSalud.Application.Models.Pacientes
{
    public class ResumenViewModel
    {
        public int IdPaciente { get; set; }
        public string Paciente { get; set; }
        public string PacienteUltimaAtencion { get; set; }
        public string Financiador { get; set; }
        public string FinanciadorPlan { get; set; }
        public string NroAfiliadoSocio { get; set; }

        public bool ShowNombre { get; set; }
        public bool ShowButton { get; set; }
        public bool ForzarParticular { get; set; }
    }
}
