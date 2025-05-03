using Dapper.Contrib.Extensions;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Vademecum")]
    public class Vademecum
    {
        [Key]
        public virtual int IdVademecum { get; set; }
        public virtual int Codigo { get; set; }
        public virtual string CodigoTroquel { get; set; }
        public virtual string PrincipioActivo { get; set; }
        public virtual string NombreComercial { get; set; }
        public virtual string Potencia { get; set; }
        public virtual int Contenido { get; set; }
        public virtual string FormaFarmaceutica { get; set; }
        public virtual string Laboratorio { get; set; }
        public virtual string Observaciones { get; set; }
    }
}
