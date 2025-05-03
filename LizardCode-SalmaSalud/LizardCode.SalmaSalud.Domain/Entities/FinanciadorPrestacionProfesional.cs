using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("FinanciadoresPrestacionesProfesionales")]
    public class FinanciadorPrestacionProfesional
    {
        [Key]
        public int IdFinanciadorPrestacionProfesional { get; set; }
        public long IdFinanciadorPrestacion { get; set; }
        public long IdProfesional { get; set; }        
        public string Codigo { get; set; }        
        public double? ValorFijo { get; set; }
        public double? Porcentaje { get; set; }        
    }
}