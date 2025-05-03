namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SaldoInicioBancoAnticipos : Entities.SaldoInicioBancoAnticipo
    {
        public string TipoSaldoInicio { get; set; }
        public string Cliente { get; set; }
        public string Proveedor { get; set; }
    }
}
