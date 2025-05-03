using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class AsientoDetalle : Entities.AsientoDetalle
    {
        public DateTime Fecha { get; set; }
        public string Codigo { get; set; }
        public string Cuenta { get; set; }

    }
}
