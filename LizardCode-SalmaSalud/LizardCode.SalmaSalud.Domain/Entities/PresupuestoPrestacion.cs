using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("PresupuestosPrestaciones")]

    public class PresupuestoPrestacion
    {
        [Key]
        public int IdPresupuestoPrestacion { get; set; }
        public int IdPresupuesto { get; set; }
        public int IdPrestacion { get; set; }
        public int Item { get; set; }
        public int Pieza { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public double Valor { get; set; }
        public double CoPago { get; set; }
    }
}
