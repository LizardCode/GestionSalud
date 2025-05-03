using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("EvolucionesOrdenes")]

    public class EvolucionOrden
    {
        [Key]
        public int IdEvolucionOrden { get; set; }
        public int IdEvolucion { get; set; }
        public DateTime Fecha { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Indicaciones { get; set; }
    }
}
