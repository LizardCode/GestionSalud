using Dapper.Contrib.Extensions;
using System;

namespace LizardCode.SalmaSalud.Domain.Entities
{
    [Table("SaldoInicioBancosCheques")]

    public class SaldoInicioBancoCheque
    {
        [Key]
        public int IdSaldoInicioCheques { get; set; }
        public int IdSaldoInicioBanco { get; set; }
        public int IdTipoSdoInicio { get; set; }
        public int? IdBanco { get; set; }
        public int IdEmpresa { get; set; }
        public string Banco { get; set; }
        public int IdTipoCheque { get; set; }
        public string NumeroCheque { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaDiferido { get; set; }
        public double Importe { get; set; }
        public int IdCheque { get; set; }

    }
}
