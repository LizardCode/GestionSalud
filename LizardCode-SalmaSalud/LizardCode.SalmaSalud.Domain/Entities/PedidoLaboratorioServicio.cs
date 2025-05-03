using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("PedidosLaboratoriosServicios")]

    public class PedidoLaboratorioServicio
    {
        [Key]
        public int IdPedidoLaboratorioServicio { get; set; }
        public int IdPedidoLaboratorio { get; set; }
        public int Item { get; set; }
        public string Servicio { get; set; }
        public double Valor { get; set; }
    }
}