using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Net.Http;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Prestaciones")]
    public class Prestacion
    {
        [Key]
        public int IdPrestacion { get; set; }
        public int IdEmpresa { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public double Valor { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public virtual int IdEstadoRegistro { get; set; }

        public double? ValorFijo { get; set; }
        public double? Porcentaje { get; set; }
    }
}
