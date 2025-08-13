using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("TipoDia")]

    public class TipoDia
    {
        [Key]
        public int IdTipoDia { get; set; }
        public string Descripcion { get; set; }
        public virtual int IdEspecialidad { get; set; }
        public string Tag { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public virtual int IdEstadoRegistro { get; set; }
    }
}
