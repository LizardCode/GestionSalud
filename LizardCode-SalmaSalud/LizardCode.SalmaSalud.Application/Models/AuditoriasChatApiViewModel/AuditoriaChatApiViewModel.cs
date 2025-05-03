using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.AuditoriasChatApiViewModel
{
    public class AuditoriaChatApiViewModel
    {
        public virtual int IdAuditoria { get; set; }
        public virtual int IdEmpresa { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual int IdEvento { get; set; }
        public virtual string Telefono { get; set; }
        public virtual string Mensaje { get; set; }
        public virtual int IdEstadoChatApi { get; set; }
        public virtual string Respuesta { get; set; }
        public virtual string Id { get; set; }
        public virtual int? IdPaciente { get; set; }
        public string Paciente { get; set; }
        public string Estado { get; set; }
        public string EstadoClase { get; set; }


        public SelectList MaestroEstados { get; set; }
        public SelectList MaestroEventos { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }

    }
}
