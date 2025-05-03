using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("FinanciadoresPrestaciones")]
    public class FinanciadorPrestacion
    {
        [Key]
        public int IdFinanciadorPrestacion { get; set; }
        public long IdFinanciador { get; set; }
        public int Item { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public double Valor { get; set; }
        public int IdFinanciadorPlan { get; set; }
        public string CodigoFinanciadorPlan { get; set; }
        public int? IdPrestacion { get; set; }
        public string CodigoPrestacion { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public virtual int IdEstadoRegistro { get; set; }

        public double? ValorFijo { get; set; }
        public double? Porcentaje { get; set; }
        public double? CoPago { get; set; }
    }
}