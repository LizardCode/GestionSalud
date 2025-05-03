using System;

namespace LizardCode.SalmaSalud.Application.Models.PortalPacientes
{
    public class EvolucionesViewModel
    {
        public int IdEvolucion { get; set; }
        public DateTime Fecha { get; set; }
        public string Empresa { get; set; }
        public string Especialidad { get; set; }
    }
}
