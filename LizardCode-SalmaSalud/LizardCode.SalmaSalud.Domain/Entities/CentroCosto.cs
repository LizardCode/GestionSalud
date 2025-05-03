using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CentrosCosto")]

    public class CentroCosto
    {
        [Key]
        public int IdCentroCosto { get; set; }
        public string Descripcion { get; set; }
        public int IdEmpresa { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
