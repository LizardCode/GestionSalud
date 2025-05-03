using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("SaldoInicioBancosAnticipos")]

    public class SaldoInicioBancoAnticipo
    {
        [Key]
        public int IdSaldoInicioAnticipos { get; set; }
        public int IdSaldoInicioBanco { get; set; }
        public int IdTipoSdoInicio { get; set; }
        public DateTime Fecha { get; set; }
        public int? IdEmpresa { get; set; }
        public int? IdCliente { get; set; }
        public int? IdProveedor { get; set; }
        public string Descripcion { get; set; }
        public string Moneda { get; set; }
        public double Cotizacion { get; set; }
        public double Importe { get; set; }
        public int? IdRecibo { get; set; }
        public int? IdOrdenPago { get; set; }

    }
}
