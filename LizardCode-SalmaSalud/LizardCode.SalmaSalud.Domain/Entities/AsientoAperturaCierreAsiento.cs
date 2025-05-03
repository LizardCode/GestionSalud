using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("AsientosAperturaCierreAsientos")]

    public class AsientoAperturaCierreAsiento
    {
        [ExplicitKey]
        public int IdAsientoAperturaCierre { get; set; }
        [ExplicitKey]
        public int IdAsiento { get; set; }

    }
}
