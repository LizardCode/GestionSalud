using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Alicuotas")]

    public class Alicuota
    {
        [Key]
        public int IdAlicuota { get; set; }
        public string Descripcion { get; set; }
        public int IdTipoAlicuota { get; set; }
        public double Valor { get; set; }
        public int CodigoAFIP { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
