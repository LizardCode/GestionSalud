using LizardCode.Framework.Application.Common.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Consultorios
{
    public class ConsultorioViewModel
    {
        public int IdConsultorio { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

        public string Edificio { get; set; }

        public string Piso { get; set; }
    }
}
