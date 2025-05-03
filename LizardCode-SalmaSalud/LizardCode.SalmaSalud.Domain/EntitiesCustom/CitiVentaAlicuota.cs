namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class CitiVentaAlicuota
    {
        public string TipoComprobante { get; set; }
        public string PuntoVenta { get; set; }
        public string NroComprobante { get; set; }
        public double ImporteNetoGravado { get; set; }
        public int Alicuota { get; set; }
        public double ImpuestoLiquidado { get; set; }
    }
}
