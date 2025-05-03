using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Sucursales")]

    public class Sucursal
    {
        [Key]
        public int IdSucursal { get; set; }
        public string Descripcion { get; set; }
        public int IdEmpresa { get; set; }
        public string CodigoSucursal { get; set; }
        public string Exenta { get; set; }
        public string Webservice { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
