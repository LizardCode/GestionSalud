using Dapper.Contrib.Extensions;
namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("TipoTurnos")]

    public class TiposTurno
    {
        [Key]
        public int IdTipoTurno { get; set; }
        public string Descripcion { get; set; }
        public string Siglas { get; set; }
    }
}
