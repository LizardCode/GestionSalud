using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Domain.EntitiesCustom.ReCaptcha
{
    public class GenericResponse
    {
        public bool success { get; set; }
        public List<string> error_codes { get; set; }

    }
}
