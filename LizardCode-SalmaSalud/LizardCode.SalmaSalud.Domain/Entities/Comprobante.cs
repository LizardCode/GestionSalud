using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Comprobantes")]

    public class Comprobante
	{
        [Key]
		public int IdComprobante { get; set; }
		public string Descripcion { get; set; }
		public int Codigo { get; set; }
		public bool EsCredito { get; set; }
		public bool EsMiPymes { get; set; }
		public string TipoComprobante { get; set; }
		public string Letra { get; set; }

	}
}
