namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class PedidoLaboratorio : Entities.PedidoLaboratorio
    {
        public string Estado { get; set; }
        public string EstadoClase { get; set; }

        public string Paciente { get; set; }
        public string PacienteDocumento { get; set; }

        public string Financiador { get; set; }
        public string FinanciadorPlan { get; set; }

        public string Laboratorio { get; set; }
        public string LaboratorioCUIT { get; set; }

        public string Usuario { get; set; }
        public double Valor { get; set; }
    }
}
