using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("CuentasContables")]

    public class CuentaContable
    {
        [Key]
        public int IdCuentaContable { get; set; }
        public int IdRubroContable { get; set; }
        public int IdEmpresa { get; set; }
        public string CodigoCuenta { get; set; }
        public string Descripcion { get; set; }
        public int? IdCodigoObservacion { get; set; }
        public bool EsCtaGastos { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
