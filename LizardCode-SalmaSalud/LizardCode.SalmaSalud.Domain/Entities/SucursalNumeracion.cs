using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("SucursalesNumeracion")]
    public class SucursalNumeracion
    {
        [ExplicitKey]
        public int IdSucursal { get; set; }
        [ExplicitKey]
        public int IdComprobante { get; set; }
        public string Numerador { get; set; }
    }
}
