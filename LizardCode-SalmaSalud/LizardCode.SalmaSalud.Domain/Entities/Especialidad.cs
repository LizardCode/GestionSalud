using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Especialidades")]
    public class Especialidades
    {
        [Key]
        public int IdEspecialidad { get; set; }
        public string Descripcion { get; set; }
        public int TurnosIntervalo { get; set; }
    }
}
