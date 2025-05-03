using System;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.PortalPacientes
{
    public class TurnosSolicitudViewModel
    {
        public List<TurnoSolicitud> Turnos { get; set; } = new List<TurnoSolicitud>();
    }

    public class TurnosViewModel
    {
        public int IdTurno { get; set; }
        public DateTime Fecha { get; set; }
        public string FechaString { get; set; }
        public string Empresa { get; set; }
        public string EmpresaDireccion { get; set; }
        public string EmpresaTelefono { get; set; }
        public string EmpresaEmail { get; set; }
        public string Especialidad { get; set; }
        public string Profesional { get; set; }
        public string Observaciones { get; set; }

        public string Estado { get; set; }
        public string EstadoClase { get; set; }
        public int IdEstadoTurno { get; set; }
    }
}
