using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class AuditoriaChatApi : Entities.AuditoriaChatApi
    {
        public virtual string Evento { get; set; }
        public virtual string EventoClase { get; set; }
        public string Paciente { get; set; }
        public string Estado { get; set; }
        public string EstadoClase { get; set; }
    }
}
