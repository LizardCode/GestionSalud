namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class CitiCompraAlicuota
    {
        public string TipoComprobante { get; set; }
        public string PuntoVenta { get; set; }
        public string NroComprobante { get; set; }
        public string CodDocumentoVendedor { get; set; }
        public string NroIdentificaiconComprador { get; set; }
        public double ImporteNetoGravado { get; set; }
        public int Alicuota { get; set; }
        public double ImpuestoLiquidado { get; set; }
    }
}
