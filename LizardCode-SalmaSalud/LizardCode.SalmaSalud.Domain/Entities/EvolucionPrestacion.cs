using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("EvolucionesPrestaciones")]

    public class EvolucionPrestacion
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
        public int IdTipoPrestacion { get; set; }

        public int? IdPrestacion { get; set; }
        public string CodigoPrestacion { get; set; }

        public double? ValorFijo { get; set; }
        public double? Porcentaje { get; set; }

        public long? IdEvolucionPrestacionAsociada { get; set; }
    }
}
