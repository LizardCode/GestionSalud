using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("PedidosLaboratoriosHistorial")]
    public class PedidoLaboratorioHistorial
    {
        [Key]
        public virtual int IdPedidoLaboratorioHistorial { get; set; }
        public virtual int IdPedidoLaboratorio { get; set; }
        public virtual int IdUsuario { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual int IdEstadoPedidoLaboratorio { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual DateTime FechaEstado { get; set; }
    }
}