using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class Asiento : Entities.Asiento
    {
        public string TipoAsiento { get; set; }
        public IList<AsientoDetalle> Items { get; set; }
    }
}
