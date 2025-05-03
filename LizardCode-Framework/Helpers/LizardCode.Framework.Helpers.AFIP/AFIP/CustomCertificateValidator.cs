using NLog;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;

namespace LizardCode.Framework.Helpers.AFIP
{
    public class CustomCertificateValidator : X509CertificateValidator
    {
        private readonly Logger _logger;


        public CustomCertificateValidator(Logger logger)
        {
            _logger = logger;
        }


        public override void Validate(X509Certificate2 certificate)
            => _logger.Info($"Validación de certificado: {certificate.IssuerName} - {certificate.SubjectName}");
    }
}