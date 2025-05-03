using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Turnos")]
    public class Turno
    {
        [Key]
        public int IdTurno { get; set; }
        public virtual int IdTipoTurno { get; set; }
        public virtual int IdEmpresa { get; set; }
        public virtual int? IdEspecialidad { get; set; }
        public virtual int? IdProfesional { get; set; }
        public virtual DateTime FechaInicio { get; set; }
        public virtual DateTime FechaFin { get; set; }
        public virtual int IdEstadoTurno { get; set; }  
        public virtual int IdUsuario { get; set; }
        public virtual int? IdProfesionalTurno { get; set; }
        public virtual int IdPaciente { get; set; }

        public virtual DateTime? FechaConfirmacion { get; set; }
        public virtual DateTime? FechaRecepcion { get; set; }
        public virtual DateTime? FechaAtencion { get; set; }

        public virtual int? IdConsultorio { get; set; }
        public virtual string Consultorio { get; set; }
        public virtual string Observaciones { get; set; }

        public virtual bool ForzarParticular { get; set; }
    }
}
