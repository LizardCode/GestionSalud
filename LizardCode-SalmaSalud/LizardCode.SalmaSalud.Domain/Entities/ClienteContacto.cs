using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ClientesContactos")]
    public class ClienteContacto
    {
        [Key]
        public int IdClienteContacto { get; set; }
        public int IdCliente { get; set; }
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
