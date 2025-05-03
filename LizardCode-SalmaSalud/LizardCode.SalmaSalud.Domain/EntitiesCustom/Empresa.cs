using System;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom
{
    public class Empresa : Entities.Empresa
    {
        public string TipoIVA { get; set; }
        public DateTime? VencimientoCertificado { get; set; }
    }
}
