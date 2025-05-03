using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("AfipAuth")]
	
	public class AfipAuth
    {
		[Key]
		public int IdAuthAFIP { get; set; }
		public int IdEmpresa { get; set; }
		public DateTime GenerationTime { get; set; }
		public DateTime ExpirationTime { get; set; }
		public string Sign { get; set; }
		public string Token { get; set; }
		public string Servicio { get; set; }
        public string CUIT { get; set; }
    }
}
