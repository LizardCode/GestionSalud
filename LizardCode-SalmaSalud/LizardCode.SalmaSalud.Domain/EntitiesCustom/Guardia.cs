using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class Guardia : Entities.Guardia
    {
        public string Profesional { get; set; }
        public string Estado { get; set; }
        public string EstadoClase { get; set; }
        public string DescripcionLiquidacion { get; set; }
    }
}
