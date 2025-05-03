using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Feriados")]
    public class Feriado
    {
        [Key]
        public virtual int IdFeriado { get; set; }
        public virtual int IdEmpresa { get; set; }
        public virtual int IdTipoFeriado { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual string Nombre { get; set; }
        public virtual string Descripcion { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public virtual int IdEstadoRegistro { get; set; }
    }
}
