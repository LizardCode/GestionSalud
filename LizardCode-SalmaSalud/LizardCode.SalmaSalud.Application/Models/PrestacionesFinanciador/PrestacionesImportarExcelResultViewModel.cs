using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.PrestacionesFinanciador
{
    public class PrestacionesImportarExcelResultViewModel
    {
        public int Cantidad { get; set; }
        public int Procesados { get; set; }
        public int Actualizados { get; set; }
        public int Nuevos { get; set; }
    }
}
