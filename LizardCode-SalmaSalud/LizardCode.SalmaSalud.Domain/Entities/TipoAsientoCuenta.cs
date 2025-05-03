using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("TipoAsientosCuentas")]
    public class TipoAsientoCuenta
    {
        [Key]
        public int IdTipoAsientoCuenta { get; set; }

        public long IdTipoAsiento { get; set; }

        public int IdCuentaContable { get; set; }

        public string Descripcion { get; set; }

        public int Item { get; set; }
    }
}
