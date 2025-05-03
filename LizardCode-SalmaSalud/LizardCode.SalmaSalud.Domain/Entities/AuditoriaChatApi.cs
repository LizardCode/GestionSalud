using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("AuditoriasChatApi")]
    public class AuditoriaChatApi
    {
        [Key]
        public virtual int IdAuditoria { get; set; }
        public virtual int? IdEmpresa { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual int IdEvento { get; set; }
        public virtual string Telefono { get; set; }
        public virtual string Mensaje { get; set; }
        public virtual int IdEstadoAuditoriaChatApi { get; set; }
        public virtual string Respuesta { get; set; }
        public virtual string Id { get; set; }

        public virtual int? IdPaciente { get; set; }
    }
}
