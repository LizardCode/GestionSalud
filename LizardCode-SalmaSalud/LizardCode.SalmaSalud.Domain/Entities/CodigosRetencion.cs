using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CodigosRetencion")]
    public class CodigosRetencion
    {
        [Key]
        public int IdCodigoRetencion { get; set; }
        public string Descripcion { get; set; }
        public int IdTipoRetencion { get; set; }
        public string Regimen { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }
    }
}
