using LizardCode.Framework.Application.Common.Enums;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.Menu
{
    public class ItemMenu
    {
        public TipoMenuItem Tipo { get; private set; }

        public string Texto { get; set; }

        public string Icono { get; set; }

        public string Action { get; set; }

        public string Codigo { get; set; }

        public bool MainSection { get; set; } = false;

        public List<ItemMenu> SubMenu { get; set; }

        public ItemMenu(TipoMenuItem type = TipoMenuItem.Item, bool mainSection = false)
        {
            Tipo = type;
            MainSection = mainSection;
        }

        public string Controller { get; set; }
    }
}