using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("AuditoriaLogin")]

    public class AuditoriaLogin
    {
        [Key]
        public int IdAuditoria { get; set; }
        public int IdUsuario { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime Fecha { get; set; }
        public string Platform { get; set; }
        public string Os { get; set; }
        public string Browser { get; set; }
        public string Version { get; set; }
        public string Resolucion { get; set; }
        public string IP { get; set; }

    }
}
