namespace LizardCode.SalmaSalud.API.Models
{
    public class SolicitarModel
    {
        public string Documento { get; set; }
        public string Telefono { get; set; }
        public int IdEspecialidad { get; set; }
        public List<int> Dias { get; set; }
        public List<int> RangosHorarios { get; set; }
    }
}
