using LizardCode.Framework.Application.Common.Annotations;

namespace LizardCode.SalmaSalud.Application.Models.RubrosArticulos
{
    public class RubrosArticulosViewModel
    {
        public int IdRubroArticulo { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

    }
}