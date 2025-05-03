using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Submenu")]

    public class Submenu
    {
        [Key]
        public int IdSubmenu { get; set; }
        public int IdMenu { get; set; }
        public string Descripcion { get; set; }
        public string Accion { get; set; }
        public int Orden { get; set; }
        public bool NoMostrar { get; set; }
        public bool EsReporte { get; set; }
    }
}
