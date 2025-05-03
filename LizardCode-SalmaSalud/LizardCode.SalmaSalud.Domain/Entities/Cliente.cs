using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }
        public string RazonSocial { get; set; }
        public string NombreFantasia { get; set; }
        public int IdTipoTelefono { get; set; }
        public int IdTipoDocumento { get; set; }
        public string Documento { get; set; }
        public int IdTipoIVA { get; set; }
        public string CUIT { get; set; }
        public string NroIBr { get; set; }
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

        public int? IdPaciente { get; set; }

        public int? IdFinanciador { get; set; }
    }
}
