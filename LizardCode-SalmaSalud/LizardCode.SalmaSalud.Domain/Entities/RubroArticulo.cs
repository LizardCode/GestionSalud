using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("RubrosArticulos")]

    public class RubroArticulo
    {
        [Key]
        public int IdRubroArticulo { get; set; }
        public string Descripcion { get; set; }
        public int IdEmpresa { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
