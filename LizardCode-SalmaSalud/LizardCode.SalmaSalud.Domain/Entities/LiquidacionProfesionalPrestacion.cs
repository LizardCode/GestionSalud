using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("LiquidacionesProfesionalesPrestaciones")]

    public class LiquidacionProfesionalPrestacion
    {
        [Key]
        public int IdLiquidacionProfesionalPrestacion { get; set; }
        public int IdLiquidacion { get; set; }
        public int? IdPrestacion { get; set; }
        public int? IdOtraPrestacion { get; set; }
        public string Descripcion { get; set; }        
        public double Valor { get; set; }
        public double ValorFijo { get; set; }
        public double Porcentaje { get; set; }
        public double ValorPorcentaje { get; set; }
        public double Total { get; set; }
        public int? IdGuardia { get; set; }
    }
}
