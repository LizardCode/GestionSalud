using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("Transferencias")]

    public class Transferencia
    {
        [Key]
        public int IdTransferencia { get; set; }
        public DateTime Fecha { get; set; }
        public string NroTransferencia { get; set; }
        public int IdEmpresa { get; set; }
        public int? IdBanco { get; set; }
        public string BancoOrigen { get; set; }
        public double Importe { get; set; }

    }
}
