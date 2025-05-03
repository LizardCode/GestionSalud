using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("EvolucionesArchivos")]

    public class EvolucionArchivo
    {
        [Key]
        public int IdEvolucionPrestacion { get; set; }
        public int IdEvolucion { get; set; }
        public int IdArchivo{ get; set; }
    }
}
