using Dapper.Contrib.Extensions;

namespace LizadCode.SalmaSalud.Notifications.Domain.Entities
{
    [Table("Turnos")]
    public class Turno
    {
        [Key]
        public int IdTurno { get; set; }
        public virtual int IdEmpresa { get; set; }
        public virtual int IdEspecialidad { get; set; }
        public virtual int IdProfesional { get; set; }
        public virtual DateTime FechaInicio { get; set; }
        public virtual DateTime FechaFin { get; set; }
        public virtual int IdEstadoTurno { get; set; }  
        public virtual int IdUsuario { get; set; }
        public virtual int? IdProfesionalTurno { get; set; }
        public virtual int IdPaciente { get; set; }

        public virtual DateTime? FechaConfirmacion { get; set; }
        public virtual DateTime? FechaRecepcion { get; set; }
        public virtual DateTime? FechaAtencion { get; set; }

        public virtual string Consultorio { get; set; }
        public virtual string Observaciones { get; set; }
    }
}
