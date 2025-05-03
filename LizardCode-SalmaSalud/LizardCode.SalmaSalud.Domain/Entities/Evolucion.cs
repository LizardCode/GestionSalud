using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Evoluciones")]

    public class Evolucion
    {
        [Key]
        public int IdEvolucion { get; set; }
        public DateTime Fecha { get; set; }
        public int IdEmpresa { get; set; }
        public int? IdTurno { get; set; }
        public int IdPaciente { get; set; }
        public int IdProfesional { get; set; }
        public int IdEspecialidad { get; set; }
        public string Diagnostico { get; set; }
        public string Observaciones { get; set; }
        public int IdEstadoRegistro { get; set; }
        public int IdEstadoEvolucion { get; set; }
        public int? IdFinanciador { get; set; }
        public int? IdFinanciadorPlan { get; set; }
        public string FinanciadorNro { get; set; }
    }
}
