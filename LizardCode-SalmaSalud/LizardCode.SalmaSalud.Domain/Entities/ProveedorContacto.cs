using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ProveedoresContactos")]
    public class ProveedorContacto
    {
        [Key]
        public int IdProveedorContacto { get; set; }
        public int IdProveedor { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cargo { get; set; }
        public int IdTipoTelefono { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }
    }
}
