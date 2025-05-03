using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("ProfesionalesTurnos")]
    public class ProfesionalTurno
    {
        [Key]
        public virtual int IdProfesionalTurno { get; set; }
        public virtual int IdEmpresa { get; set; }
        public virtual int IdProfesional { get; set; }
        public virtual int IdEspecialidad { get; set; }
        public virtual DateTime FechaInicio { get; set; }
        public virtual DateTime FechaFin { get; set; }
        public virtual int IdEstadoProfesionalTurno { get; set; }
    }
}
