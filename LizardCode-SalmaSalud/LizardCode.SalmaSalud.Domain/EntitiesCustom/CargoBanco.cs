using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class CargoBanco : Entities.CargoBanco
    {
        public string Banco { get; set; }

        public IList<CargoBancoItem> Items { get; set; }
    }
}
