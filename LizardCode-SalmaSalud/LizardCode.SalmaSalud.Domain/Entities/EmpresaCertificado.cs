using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("EmpresasCertificados")]
    public class EmpresaCertificado
    {
        [Key]
        public int IdEmpresaCertificado { get; set; }
        public int IdEmpresa { get; set; }
        public string CRT { get; set; }
        public DateTime Vencimiento { get; set; }
    }
}
