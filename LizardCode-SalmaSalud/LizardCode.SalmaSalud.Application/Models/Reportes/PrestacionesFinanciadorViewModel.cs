using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Reportes
{
    public class PrestacionesFinanciadorViewModel
    {
        public int IdFinanciador { get; set; }
        public int IdTipoPrestacion { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public SelectList MaestroFinanciadores { get; set; }
        public SelectList MaestroTiposPrestacion { get; set; }
    }
}
