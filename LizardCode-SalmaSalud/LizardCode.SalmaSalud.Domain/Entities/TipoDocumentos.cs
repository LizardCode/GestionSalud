using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("tipodocumentos")]

    public class TipoDocumentos
    {
        [Key]
        public int IdTipoDocumento { get; set; }
        public string Descripcion { get; set; }
    }
}
