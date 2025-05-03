using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Empresas")]
    public class Empresa
    {
        [Key]
        public int IdEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public string NombreFantasia { get; set; }
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
        public string CSR { get; set; }
        public string PrivateKey { get; set; }
        public bool AgentePercepcionAGIP { get; set; }
        public bool AgentePercepcionARBA { get; set; }
        public bool EnableProdAFIP { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

        public DateTime FechaInicioActividades { get; set; }

        public string TurnosHoraInicio { get; set; }
        public string TurnosHoraFin { get; set; }
        public int TurnosIntervalo { get; set; }

        public int? IdArchivoRecetaLogo { get; set; }
    }
}
