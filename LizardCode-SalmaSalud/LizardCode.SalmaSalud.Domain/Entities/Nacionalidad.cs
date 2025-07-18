using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Nacionalidades")]

    public class Nacionalidad
    {
        [Key]
        public int IdNacionalidad { get; set; }
        public string Descripcion { get; set; }
    }
}