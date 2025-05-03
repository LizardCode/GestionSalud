using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("PedidosLaboratorios")]

    public class PedidoLaboratorio
    {
        [Key]
        public int IdPedidoLaboratorio { get; set; }        
        public int IdPresupuesto { get; set; }
        public int IdLaboratorio { get; set; }
        public int? IdFinanciador { get; set; }
        public int? IdFinanciadorPlan { get; set; }
        public string FinanciadorNro { get; set; }
        public DateTime Fecha { get; set; }
        public int IdEstadoPedidoLaboratorio { get; set; }
        public string Observaciones { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaEstimada { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }
        public int IdUsuario{ get; set; }
        public int IdEmpresa { get; set; }
        public int? NumeroSobre { get; set; }
    }
}