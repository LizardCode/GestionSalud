using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Profesionales")]
    public class Profesional
    {
        [Key]
        public int IdProfesional { get; set; }
        public string Nombre { get; set; }
        public string Matricula { get; set; }
        public string MatriculaProvincial { get; set; }
        public int IdEspecialidad { get; set; }
        public string Funcion { get; set; }
        public int IdTipoIVA { get; set; }
        public string CUIT { get; set; }
        public string NroIBr { get; set; }
        public int IdTipoTelefono { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        public string Piso { get; set; }
        public string Departamento { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

        public string PolizaNumero { get; set; }
        public string PolizaAseguradora { get; set; }
        public DateTime PolizaVencimiento { get; set; }

        public int CantidadSobreturnos { get; set; }
        public int TurnosIntervalo { get; set; }
        public int? IdArchivoFirma { get; set; }
    }
}
