using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class FinanciadorPrestacion : Entities.FinanciadorPrestacion
    {
        public string FinanciadorPlan { get; set; }
        public string FinanciadorPlanCodigo { get; set; }
        public string Prestacion { get; set; }
        public string PrestacionCodigo { get; set; }
    }
}
