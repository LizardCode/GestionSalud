using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class Paciente : Entities.Paciente
    {
        public string Financiador { get; set; }
        public string FinanciadorPlan { get; set; }
    }
}

