using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class Turno : Entities.Turno
    {        
        public string TipoTurno { get; set; }
        public string TipoTurnoDescripcion { get; set; }
        public string Especialidad { get; set; }
        public string Profesional { get; set; }
        public string Paciente { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Telefono { get; set; }
        public string Estado { get; set; }
        public string EstadoClase { get; set; }
        public bool FichaIncompleta { get; set; }
        public string Empresa { get; set; }
        public string EmpresaDireccion { get; set; }
        public string EmpresaTelefono { get; set; }
        public string EmpresaEmail { get; set; }
        public DateTime FechaInicio { get; set; }
        public string Financiador { get; set; }
        public string FinanciadorPlan { get; set; }
        public string Email { get; set; }
        public string Documento { get; set; }
    }
}
