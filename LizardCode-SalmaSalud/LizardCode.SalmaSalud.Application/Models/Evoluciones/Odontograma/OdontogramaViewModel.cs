using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Models.Evoluciones.Odontograma
{
	public class OdontogramaViewModel
	{
		public int IdEvolucion { get; set; }
		public List<OdontogramaPiezaViewModel> Piezas { get; set; } = new List<OdontogramaPiezaViewModel>();
	}

	public class OdontogramaPiezaViewModel
	{
		public int IdEvolucion { get; set; }
		public int Pieza { get; set; }
		public bool Caries { get; set; }
		public bool Corona { get; set; }
		public bool PrFija { get; set; }
		public bool PrRemovible { get; set; }
		public bool Amalgama { get; set; }
		public bool Ausente { get; set; }
		public bool Ortodoncia { get; set; }
		public bool Extraccion { get; set; }
        public string Observaciones { get; set; }
        public List<OdontogramaZonaViewModel> Zonas { get; set; } = new List<OdontogramaZonaViewModel>();
	}

	public class OdontogramaZonaViewModel
	{
		public int Pieza { get; set; }
		public int Zona { get; set; }
		public int TipoTrabajo { get; set; }
	}
}
