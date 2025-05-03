using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;


namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Monedas")]

    public class Moneda
    {
        [Key]
        public int IdMoneda { get; set; }
        public string Descripcion { get; set; }
        public string Simbolo { get; set; }
        public string Codigo { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
