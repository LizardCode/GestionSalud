namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ComprobanteCompraItem
    {
        public bool Seleccion { get; set; }
        public int Item { get; set; }
        public string Estimado { get; set; }
        public string Descripcion { get; set; }
        public string IdMoneda { get; set; }
        public string Moneda { get; set; }
        public int IdAlicuota { get; set; }
        public string Alicuota { get; set; }
        public double Importe { get; set; }

    }
}
