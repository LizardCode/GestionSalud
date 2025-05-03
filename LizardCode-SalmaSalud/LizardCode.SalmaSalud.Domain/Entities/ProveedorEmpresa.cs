using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ProveedoresEmpresas")]
    public class ProveedorEmpresa
    {
        [ExplicitKey]
        public virtual int IdProveedor { get; set; }
        [ExplicitKey]
        public virtual int IdEmpresa { get; set; }

    }
}
