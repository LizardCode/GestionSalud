using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class DepositoBanco : Entities.DepositoBanco
    {
        public string Banco { get; set; }

        public IList<DepositoBancoDetalle> Items { get; set; }
    }
}
