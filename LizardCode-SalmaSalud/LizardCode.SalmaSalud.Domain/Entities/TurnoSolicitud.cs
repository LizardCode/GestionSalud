﻿using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("TurnosSolicitud")]
    public class TurnoSolicitud
    {
        [Key]
        public int IdTurnoSolicitud { get; set; }
        public virtual DateTime FechaSolicitud { get; set; }
        public virtual int IdPaciente { get; set; }
        public virtual int IdEspecialidad { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual int IdEstadoTurnoSolicitud { get; set; }
        public virtual DateTime? FechaAsignacion { get; set; }
        public virtual string ObservacionesAsignacion { get; set; }
        public virtual int IdUsuarioAsignacion { get; set; }
        public virtual int IdEstadoRegistro { get; set; }
        public virtual int? IdProfesional { get; set; }        
    }
}