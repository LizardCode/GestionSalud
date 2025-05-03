using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class MayorCuentasDetalle
    {
        public DateTime? Fecha { get; set; }
        public string Descripcion { get; set; }
        public double Debitos { get; set; }
        public double Creditos { get; set; }
        public double Saldo { get; set; }
    }
}
