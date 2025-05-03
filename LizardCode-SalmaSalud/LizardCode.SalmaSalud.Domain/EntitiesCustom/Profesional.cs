using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class Profesional : Entities.Profesional
    {
        public string Especialidad { get; set; }
        public string TipoMatricula { get; set; }
        public string TipoIVA { get; set; }
    }
}
