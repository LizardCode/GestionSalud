using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("EvolucionesOtrasPrestaciones")]

    public class EvolucionOtraPrestacion
    {
        [Key]
        public int IdEvolucionPrestacion { get; set; }
        public int IdEvolucion { get; set; }
        public int Item { get; set; }
        public int Pieza { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public double Valor { get; set; }
        public int IdEstadoPrestacion { get; set; }

        public double? ValorFijo { get; set; }
        public double? Porcentaje { get; set; }
    }
}
