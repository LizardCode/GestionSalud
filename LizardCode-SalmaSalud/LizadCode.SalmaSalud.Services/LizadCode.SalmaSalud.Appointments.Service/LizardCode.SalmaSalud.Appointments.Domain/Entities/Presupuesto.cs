using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Appointments.Domain.Entities
{
    [Table("Presupuestos")]

    public class Presupuesto
    {
        [Key]
        public int IdPresupuesto { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Observaciones { get; set; }
        public int IdEmpresa { get; set; }
        public int IdPaciente { get; set; }
        public int IdEstadoRegistro { get; set; }
        public int IdEstadoPresupuesto { get; set; }
        public int? IdFinanciador { get; set; }
        public int? IdFinanciadorPlan { get; set; }
        public string FinanciadorNro { get; set; }
        public int IdUsuario { get; set; }
    }
}
