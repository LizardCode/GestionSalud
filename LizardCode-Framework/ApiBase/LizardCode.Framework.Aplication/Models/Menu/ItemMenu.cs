using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.Framework.Application.Models.Menu
{
    public class ItemMenu
    {
        public TipoMenuItem Tipo { get; private set; }

        public string Texto { get; set; }

        public string Icono { get; set; }

        public string Action { get; set; }

        public string Codigo { get; set; }


        public List<ItemMenu> SubMenu { get; set; }

        public ItemMenu(TipoMenuItem type = TipoMenuItem.Item)
        {
            Tipo = type;
        }
    }
}