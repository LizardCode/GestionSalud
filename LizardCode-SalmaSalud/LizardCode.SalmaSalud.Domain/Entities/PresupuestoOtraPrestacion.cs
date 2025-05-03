using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("PresupuestosOtrasPrestaciones")]

    public class PresupuestoOtraPrestacion
    {
        [Key]
        public int IdPresupuestoOtraPrestacion { get; set; }
        public int IdPresupuesto { get; set; }
        public int IdOtraPrestacion { get; set; }
        public int Item { get; set; }
        public int Pieza { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public double Valor { get; set; }
    }
}
