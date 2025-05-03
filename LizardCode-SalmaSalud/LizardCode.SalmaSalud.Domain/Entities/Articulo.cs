using Dapper.Contrib.Extensions;
using LizardCode.SalmaSalud.Domain.Enums;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Articulos")]

    public class Articulo
    {
        [Key]
        public int IdArticulo { get; set; }
        public int IdRubroArticulo { get; set; }
        public string CodigoBarras { get; set; }
        public string Descripcion { get; set; }
        public double Alicuota { get; set; }
        public string Detalle { get; set; }
        public double Stock { get; set; }
        public int IdEmpresa { get; set; }
        public int IdCuentaContableVentas { get; set; }
        public int IdCuentaContableCompras { get; set; }

        [SoftDelete((int)EstadoRegistro.Eliminado)]
        public int IdEstadoRegistro { get; set; }

    }
}
