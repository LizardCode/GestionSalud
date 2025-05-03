using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Guardias")]
    public class Guardia
    {
        [Key]
        public int IdGuardia { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime Fecha { get; set; }
        public int IdUsuario { get; set; }
        public int IdProfesional { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public double Total { get; set; }
        public int IdEstadoGuardia { get; set; }
        public string Observaciones { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }
    }
}