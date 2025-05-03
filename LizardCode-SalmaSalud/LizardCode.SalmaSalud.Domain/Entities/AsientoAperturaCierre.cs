using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("AsientosAperturaCierre")]
	
	public class AsientoAperturaCierre
    {
		[Key]
        public int IdAsientoAperturaCierre { get; set; }
        public int IdEmpresa { get; set; }
        public int IdEjercicio { get; set; }
        public int IdTipoAsientoAuto { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaIngreso { get; set; }


        [SoftDelete((int)EstadoRegistro.Eliminado)]
		public int IdEstadoRegistro { get; set; }
	}
}
