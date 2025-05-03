using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("PrestacionesProfesionales")]
    public class PrestacionProfesional
    {
        [Key]
        public int IdPrestacionProfesional { get; set; }
        public long IdPrestacion { get; set; }
        public long IdProfesional { get; set; }
        public string Codigo { get; set; }
        public double? ValorFijo { get; set; }
        public double? Porcentaje { get; set; }
    }
}