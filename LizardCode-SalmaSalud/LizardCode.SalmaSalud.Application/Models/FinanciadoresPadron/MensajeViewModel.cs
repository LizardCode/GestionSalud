using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.FinanciadoresPadron
{
    public class MensajeViewModel
    {
        public string MensajePacienteFinanciadorNro { get; set; }
        public string MensajePadronFinanciadorNro { get; set; }

        public bool MensajeForzarParticular { get; set; } = true;
        public bool MensajeForzarPadron { get; set; } = false;
        public bool UsaPacienteFinanciadorNro { get; set; }
        public bool UsaPadronFinanciadorNro { get; set; }
    }
}
