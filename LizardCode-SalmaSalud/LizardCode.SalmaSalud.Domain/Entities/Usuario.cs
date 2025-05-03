using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public virtual int IdUsuario { get; set; }
        public virtual int IdTipoUsuario { get; set; }
        public virtual string Email { get; set; }
        public virtual string Login { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Password { get; set; }
        public virtual string PasswordSalt { get; set; }
        public virtual DateTime Vencimiento { get; set; }
        public virtual bool Admin { get; set; }
        public virtual string BlankToken { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public virtual int IdEstadoRegistro { get; set; }
        public virtual int? IdProfesional { get; set; }
        public virtual int? IdPaciente { get; set; }
    }
}
