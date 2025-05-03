using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("RubrosContables")]

    public class RubroContable
    {
        [Key]
        public int IdRubroContable { get; set; }
        public string CodigoRubro { get; set; }
        public string Descripcion { get; set; }
        public int? IdRubroPadre { get; set; }
        public int IdEmpresa { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
