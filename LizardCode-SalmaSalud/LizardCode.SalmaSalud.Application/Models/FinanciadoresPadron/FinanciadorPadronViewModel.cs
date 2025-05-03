using LizardCode.Framework.Application.Common.Annotations;
using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LizardCode.SalmaSalud.Application.Models.FinanciadoresPadron
{
    public class FinanciadorPadronViewModel
    {
        public int IdFinanciadorPadron { get; set; }
        public int IdFinanciador { get; set; }

        [RequiredEx]
        [Remote("ValidarCodigo", "PrestacionesFinanciador", AdditionalFields = "IdFinanciadorPrestacion,IdFinanciadorPlan", HttpMethod = "Post", ErrorMessage = "Código en uso")]
        public string Codigo { get; set; }

        [RequiredEx]
        [MaskConstraint(MaskConstraintType.Custom, blocks: "[8]", numericOnly: true)]
        [Remote("ValidarDocumento", "FinanciadoresPadron", AdditionalFields = "IdFinanciadorPadron", HttpMethod = "Post", ErrorMessage = "Documento existente")]
        public string Documento { get; set; }

        [RequiredEx]
        public string FinanciadorNro { get; set; }

        public string Nombre { get; set; }

        public IFormFile FileExcel { get; set; }
    }
}