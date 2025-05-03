using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("TipoAsientos")]

    public class TipoAsiento
    {
        [Key]
        public int IdTipoAsiento { get; set; }
        public int IdEmpresa { get; set; }
        public string Descripcion { get; set; }
        
        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
