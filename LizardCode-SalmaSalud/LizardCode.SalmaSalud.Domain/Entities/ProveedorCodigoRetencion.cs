using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ProveedoresCodigosRetencion")]
    public class ProveedorCodigoRetencion
    {
        [ExplicitKey]
        public virtual int IdProveedor { get; set; }
        [ExplicitKey]
        public virtual int IdCodigoRetencion { get; set; }

    }
}
