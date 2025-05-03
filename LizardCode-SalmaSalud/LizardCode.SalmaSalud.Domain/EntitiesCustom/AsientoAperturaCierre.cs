using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class AsientoAperturaCierre : Entities.AsientoAperturaCierre
    {
        public string TipoAsiento { get; set; }

        public string Ejercicio { get; set; }

        public IList<AsientoDetalle> Items { get; set; }
    }
}
