using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("TipoRangoHorario")]

    public class TipoRangoHorario
    {
        [Key]
        public int IdRangoHorario { get; set; }
        public string Descripcion { get; set; }
        public virtual int IdEspecialidad { get; set; }
    }
}