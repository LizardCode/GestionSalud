using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
	[Table("EvolucionesOdontogramasPiezas")]
	public class EvolucionOdontogramaPieza
	{
		[Key]
		public int IdEvolucionOdontogramaPieza { get; set; }
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
    }

	[Table("EvolucionesOdontogramasPiezasZonas")]
	public class EvolucionOdontogramaPiezaZona
	{
		[Key]
		public int IdEvolucionOdontogramaPiezaZona { get; set; }
		public int IdEvolucionOdontogramaPieza { get; set; }
		public int IdEvolucion { get; set; }
		public int Pieza { get; set; }
		public int Zona { get; set; }
		public int IdTipoTrabajoOdontograma { get; set; }
	}
}
