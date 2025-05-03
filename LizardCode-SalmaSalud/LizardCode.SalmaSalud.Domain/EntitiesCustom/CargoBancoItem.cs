namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class CargoBancoItem : Entities.CargoBancoItem
    {
        public string Codigo { get; set; }
        public string Cuenta { get; set; }

        public double IdAlicuota { get; set; }
    }
}
