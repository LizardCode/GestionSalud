using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("LiquidacionesProfesionales")]

    public class LiquidacionProfesional
    {
        [Key]
        public int IdLiquidacionProfesional { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string Observaciones { get; set; }
        public int IdEmpresa { get; set; }
        public int IdProfesional { get; set; }
        public int IdUsuario { get; set; }
        public double SubTotal { get; set; }
        public double Redondeo { get; set; }
        public double Total { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

        public int IdEstadoLiquidacionProfesional { get; set; }
    }
}
