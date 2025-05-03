using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ClientesEmpresas")]
    public class ClienteEmpresa
    {
        [ExplicitKey]
        public virtual int IdCliente { get; set; }
        [ExplicitKey]
        public virtual int IdEmpresa { get; set; }

    }
}
