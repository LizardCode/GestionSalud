using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Consultorios")]
    public class Consultorio
    {
        [Key]
        public virtual int IdConsultorio { get; set; }
        public virtual int IdEmpresa { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Edificio { get; set; }
        public virtual string Piso { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public virtual int IdEstadoRegistro { get; set; }
    }
}