using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("FinanciadoresPlanes")]

    public class FinanciadorPlan
    {
        [Key]
        public int IdFinanciadorPlan { get; set; }
        public int IdFinanciador { get; set; }
        public int Item { get; set; }
        public string Nombre { get; set; }
        public string Observaciones { get; set; }
        public string Codigo { get; set; }
    }
}
