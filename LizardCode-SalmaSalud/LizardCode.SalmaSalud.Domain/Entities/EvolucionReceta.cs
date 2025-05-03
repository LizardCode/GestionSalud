using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("EvolucionesRecetas")]

    public class EvolucionReceta
    {
        [Key]
        public int IdEvolucionReceta { get; set; }
        public int IdEvolucion { get; set; }
        public DateTime Fecha { get; set; }
        public virtual int IdVademecum { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual int Cantidad { get; set; }
        public virtual string Dosis { get; set; }
        public virtual string Frecuencia { get; set; }
        public virtual string Indicaciones { get; set; }
        public virtual int Codigo { get; set; }
        public virtual string CodigoTroquel { get; set; }
        public virtual string PrincipioActivo { get; set; }
        public virtual string NombreComercial { get; set; }
        public virtual string Potencia { get; set; }
        public virtual int Contenido { get; set; }
        public virtual string FormaFarmaceutica { get; set; }
        public virtual string Laboratorio { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual bool NoSustituir { get; set; }
    }
}
