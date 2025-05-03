using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("TurnosHistorial")]
    public class TurnoHistorial
    {
        [Key]
        public virtual int IdTurnoHistorial { get; set; }
        public virtual int IdTurno { get; set; }
        public virtual int IdUsuario { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual int IdEstadoTurno { get; set; }
        public virtual string Observaciones { get; set; }
    }
}
