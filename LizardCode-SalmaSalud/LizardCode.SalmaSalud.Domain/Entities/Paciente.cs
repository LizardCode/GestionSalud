using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Pacientes")]
    public class Paciente
    {
        [Key]
        public int IdPaciente { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Documento { get; set; }
        public int IdTipoTelefono { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Nacionalidad { get; set; }
        public int? IdFinanciador { get; set; }
        public int? IdFinanciadorPlan { get; set; }
        public string FinanciadorNro { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

        public DateTime? UltimaAtencion { get; set; }
    }
}
