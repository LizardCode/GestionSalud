using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("TurnosSolicitudRangoHorario")]
    public class TurnoSolicitudRangoHorario
    {
        [Key]
        public int IdTurnoSolicitudDia { get; set; }
        public int IdTurnoSolicitud { get; set; }
        public virtual int IdRangoHorario { get; set; }
    }
}
