using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("SaldoCtaCteCli")]
    public class SdoCtaCteCli
    {
        [Key]
        public int IdSaldoCtaCteCli { get; set; }

        public int IdUsuario { get; set; }

        public int IdEmpresa { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }

        public string Descripcion { get; set; }

        public int IdEstadoRegistro { get; set; }

        public double Total { get; set; }
    }
}
