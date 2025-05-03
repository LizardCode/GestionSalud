using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("UsuariosEmpresas")]
    public class UsuarioEmpresa
    {
        [ExplicitKey]
        public virtual int IdUsuario { get; set; }
        [ExplicitKey]
        public virtual int IdEmpresa { get; set; }

    }
}
