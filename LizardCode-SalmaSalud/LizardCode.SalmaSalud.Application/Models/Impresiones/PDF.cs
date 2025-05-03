namespace LizardCode.SalmaSalud.Application.Models.Impresiones
{
    public class PDF
    {
        public string Filename { get; set; }
        public int Length { get; set; }
        public byte[] Content { get; set; }
    }
}
