namespace LizardCode.Framework.Application.Models.ReCaptcha
{
    public class GenericResponse
    {
        public bool success { get; set; }
        public List<string> error_codes { get; set; }

    }
}
