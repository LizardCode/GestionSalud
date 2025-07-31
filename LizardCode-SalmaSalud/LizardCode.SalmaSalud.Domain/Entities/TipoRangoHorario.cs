using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("TipoRangoHorario")]

    public class TipoRangoHorario
    {
        [Key]
        public int IdRangoHorario { get; set; }
        public string Descripcion { get; set; }
        public virtual int IdEspecialidad { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public virtual int IdEstadoRegistro { get; set; }
    }
}