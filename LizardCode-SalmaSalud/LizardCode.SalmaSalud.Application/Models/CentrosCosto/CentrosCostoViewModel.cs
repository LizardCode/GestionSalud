using LizardCode.Framework.Application.Common.Annotations;

namespace LizardCode.SalmaSalud.Application.Models.CentrosCosto
{
    public class CentrosCostoViewModel
    {
        public int IdCentroCosto { get; set; }

        [RequiredEx]
        public string Descripcion { get; set; }

    }
}