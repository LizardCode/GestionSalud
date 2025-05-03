using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("SaldoInicioBancos")]

    public class SaldoInicioBanco
    {
        [Key]
        public int IdSaldoInicioBanco { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public int IdEmpresa { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaIngreso { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
