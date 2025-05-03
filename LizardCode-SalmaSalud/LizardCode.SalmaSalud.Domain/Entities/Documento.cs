using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Documentos")]

    public class Documento
    {
        [Key]
        public int IdDocumento { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVto { get; set; }
        public string NroDocumento { get; set; }
        public double Importe { get; set; }
        public string Firmante { get; set; }
        public string CUITFirmante { get; set; }

    }
}
