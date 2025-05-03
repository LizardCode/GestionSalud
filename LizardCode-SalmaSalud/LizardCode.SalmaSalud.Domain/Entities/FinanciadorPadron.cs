using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("FinanciadoresPadron")]
    public class FinanciadorPadron
    {
        [Key]
        public int IdFinanciadorPadron { get; set; }
        public int IdFinanciador { get; set; }
        public string Documento { get; set; }
        public string FinanciadorNro { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }
    }
}
