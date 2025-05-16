using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("TurnosSolicitudDia")]
    public class TurnoSolicitudDia
    {
        [Key]
        public int IdTurnoSolicitudDia { get; set; }
        public int IdTurnoSolicitud { get; set; }
        public virtual int Dia { get; set; }        
    }
}
