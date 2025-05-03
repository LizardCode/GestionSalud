using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Presupuestos
{
    public class PresupuestosAprobadosDetalleViewModel
    {
        public List<PresupuestoPrestacionViewModel> Prestaciones { get; set; }
        public List<PresupuestoOtraPrestacionViewModel> OtrasPrestaciones { get; set; }
    }
}
