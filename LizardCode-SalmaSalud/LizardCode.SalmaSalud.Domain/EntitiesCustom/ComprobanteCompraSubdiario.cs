namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ComprobanteCompraSubdiario : Entities.ComprobanteCompra
    {
        public string Comprobante { get; set; }
        public string Proveedor { get; set; }
        public string CUIT { get; set; }
        public string TipoIVA { get; set; }
        public double Alicuota { get; set; }
        public double Neto { get; set; }
        public double NoGravado { get; set; }
        public double ImporteAlicuota { get; set; }
        public double PercepcionIVA { get; set; }
        public double PercepcionIB { get; set; }
        public double ImporteTotal { get; set; }

}
}
