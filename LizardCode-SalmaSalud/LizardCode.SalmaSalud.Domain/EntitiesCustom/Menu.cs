namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class Menu : Entities.Menu
    {
        public int IdSubmenu { get; set; }
        public string DescripcionSubmenu { get; set; }
        public string Accion { get; set; }
        public bool EsReporte { get; set; }
    }
}
