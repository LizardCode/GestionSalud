using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class OrdenPagoPlanillaGasto
	{
        public bool Seleccionar { get; set; }
		public int IdPlanillaGastos { get; set; }
		public DateTime Fecha { get; set; }
		public string Descripcion { get; set; }
		public double Importe { get; set; }

	}
}
