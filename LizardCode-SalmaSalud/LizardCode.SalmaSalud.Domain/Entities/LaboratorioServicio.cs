using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("LaboratoriosServicios")]
    public class LaboratorioServicio
    {
        [Key]
        public int IdLaboratorioServicio { get; set; }
        public long IdProveedor { get; set; }
        public int Item { get; set; }
        public string Descripcion { get; set; }
        public double Valor { get; set; }        
    }
}