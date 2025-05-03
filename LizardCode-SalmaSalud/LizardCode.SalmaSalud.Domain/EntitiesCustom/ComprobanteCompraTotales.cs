namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class ComprobanteCompraTotales
    {
        public int IdComprobanteCompra { get; set; }
        public int IdComprobante { get; set; }
        public bool EsCredito { get; set; }
        public double Neto { get; set; }
        public double Impuestos { get; set; }
        public double ImportePagoTotal { get; set; }
    }
}
