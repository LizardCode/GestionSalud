using System.ComponentModel;

namespace Template.Application.Common.Enums
{
    public enum TipoUsuario
    {
        [Description("ADMINISTRADOR")]
        Admin = 1,
        [Description("RECEPCIONISTA")]
        Recepcionista = 2,
        [Description("MEDICO")]
        Medico = 3
    }
}
