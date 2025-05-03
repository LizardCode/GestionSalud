using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class SaldoInicioBanco : Entities.SaldoInicioBanco
    {
        public List<Entities.SaldoInicioBancoCheque> Cheques { get; set; }
        public List<Entities.SaldoInicioBancoAnticipo> AnticiposClientes { get; set; }
        public List<Entities.SaldoInicioBancoAnticipo> AnticiposProveedores { get; set; }
    }
}
