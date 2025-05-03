namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class Cheque : Entities.Cheque
    {
        public string EstadoCheque { get; set; }
        public string TipoCheque { get; set; }
        public int? IdAsiento { get; set; }
    }
}
