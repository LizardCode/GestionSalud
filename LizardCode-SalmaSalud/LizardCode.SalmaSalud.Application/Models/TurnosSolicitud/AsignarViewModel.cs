using LizardCode.Framework.Application.Common.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.TurnosSolicitud
{
    public class AsignarViewModel
    {
        public int IdTurnoSolicitud { get; set; }

        [RequiredEx]
        public DateTime? Fecha { get; set; }

        public string Observaciones { get; set; }
    }
}
