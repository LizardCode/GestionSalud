using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Presupuestos
{
    public class PresupuestoAprobadoViewModel
    {
        public List<PresupuestoAprobadoDetalleViewModel> Presupuestos { get; set; }
    }

    public class PresupuestoAprobadoDetalleViewModel
    {
        public int IdPresupuesto { get; set; }
        public DateTime Fecha { get; set; }

        public double TotalCoPagos { get; set; }
        public double TotalPrestaciones { get; set; }
        public double Total { get; set; }
    }
}
