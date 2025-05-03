using Dapper.Contrib.Extensions;
using System;


namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("MonedasFechasCambio")]

    public class MonedaFechasCambio
    {
        [Key]
        public int IdMonedaFecha { get; set; }
        public int IdMoneda { get; set; }
        public DateTime Fecha { get; set; }
        public double Cotizacion { get; set; }
        public int IdEmpresa { get; set; }

    }
}
