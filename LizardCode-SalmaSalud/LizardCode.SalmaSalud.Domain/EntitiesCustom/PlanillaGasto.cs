using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class PlanillaGastos : Entities.PlanillaGastos
    {
        public int? Anno { get; set; }
        public int? Mes { get; set; }
        public int? Numero { get; set; }

        public List<EntitiesCustom.PlanillaGastoItem> Items { get; set; }
    }
}
