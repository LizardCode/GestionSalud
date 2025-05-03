using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.SalmaSalud.Application.Models.ListadoEstimados
{
    public class ListadoEstimadosViewModel
    {
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string Tema { get; set; }
        public int IdCliente { get; set; }
        public int IdEstadoItemVenta { get; set; }
        public int IdEstadoItemCosto { get; set; }
        public SelectList MaestroClientes { get; set; }
        public SelectList MaestroEstadosItemVenta { get; set; }
        public SelectList MaestroEstadosItemCosto { get; set; }

    }
}