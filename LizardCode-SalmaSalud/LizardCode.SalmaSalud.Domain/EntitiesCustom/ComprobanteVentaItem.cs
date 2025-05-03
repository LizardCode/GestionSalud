namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ComprobanteVentaItem
    {
        public bool Seleccion { get; set; }
        public int Item { get; set; }
        public string Descripcion { get; set; }
        public string IdMoneda { get; set; }
        public string Moneda { get; set; }
        public string Alicuota { get; set; }
        public double Importe { get; set; }

        public int? IdEvolucionPrestacion { get; set; }
        public int? IdEvolucionOtraPrestacion { get; set; }
    }
}
