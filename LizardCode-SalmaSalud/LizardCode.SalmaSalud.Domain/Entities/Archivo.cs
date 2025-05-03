using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Archivos")]

    public class Archivo
    {
        [Key]
        public int IdArchivo { get; set; }
        public DateTime Fecha { get; set; }
        public string Nombre { get; set; }
        public string Extension { get; set; }
        public string Tipo { get; set; }
        public byte[] Contenido { get; set; }
        public string Url { get; set; }
    }
}
