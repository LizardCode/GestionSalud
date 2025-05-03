using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class TurnoSolicitud : Entities.TurnoSolicitud
    {
        public string Estado { get; set; }
        public string EstadoClase { get; set; }
        public string UsuarioAsignacion { get; set; }

        public string Especialidad { get; set; }
        public string Paciente { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Documento { get; set; }
    }
}
