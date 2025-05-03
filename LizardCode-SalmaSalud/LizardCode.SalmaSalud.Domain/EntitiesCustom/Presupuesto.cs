using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class Presupuesto : Entities.Presupuesto
    {
        public string Estado { get; set; }
        public string EstadoClase { get; set; }
        public string Paciente { get; set; }
        public string PacienteDocumento { get; set; }
        public string Usuario { get; set; }

        public double TotalCoPagos { get; set; }
        public double TotalPrestaciones { get; set; }
        public double Total { get; set; }

        public bool EnPedido { get; set; }
    }
}
