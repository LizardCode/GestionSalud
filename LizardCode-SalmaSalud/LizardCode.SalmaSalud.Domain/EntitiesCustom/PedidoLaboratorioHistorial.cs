using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class PedidoLaboratorioHistorial : Entities.PedidoLaboratorioHistorial
    {
        public string Estado { get; set; }
        public string EstadoClase { get; set; }

        public string Usuario { get; set; }
    }
}
