using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CierreMes")]

    public class CierreMes
	{
        [Key]
		public int IdCierre { get; set; }
		public int IdEmpresa { get; set; }
		public int IdEjercicio { get; set; }
		public int Anno { get; set; }
		public int Mes { get; set; }
		public string Modulo { get; set; }
		public string Cierre { get; set; }

	}
}
