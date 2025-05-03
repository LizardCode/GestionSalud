using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Bancos")]

    public class Banco
    {
        [Key]
        public int IdBanco { get; set; }
        public int IdEmpresa { get; set; }
        public string Descripcion { get; set; }
        public string CUIT { get; set; }
        public string NroCuenta { get; set; }
        public string CBU { get; set; }
        public double SaldoDescubierto { get; set; }
        public int? IdProveedor { get; set; }
        public bool EsDefault { get; set; }
        public int IdCuentaContable { get; set; }
        public int IdMoneda { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
