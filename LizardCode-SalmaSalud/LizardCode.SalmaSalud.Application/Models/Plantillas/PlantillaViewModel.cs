using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LizardCode.Framework.Application.Common.Annotations;

namespace LizardCode.SalmaSalud.Application.Models.Plantillas
{
    public class PlantillaViewModel
    {
        public int IdPlantilla { get; set; }

        [RequiredEx]
        [Remote("ValidarTipoPlantilla", "Plantillas", AdditionalFields = "IdPlantilla", ErrorMessage = "Existe una Plantilla de este tipo dada de Alta.", HttpMethod = "POST")]
        public int IdTipoPlantilla { get; set; }

        [RequiredEx]
        [StringLengthEx(250)]
        public string Descripcion { get; set; }

        [RequiredEx]
        public string Texto { get; set; }

        //public Dictionary<string, string> EtiquetasPlanilla { get; set; }

        public SelectList MaestroTiposPlantilla { get; set; }
        public SelectList MaestroEtiquetas { get; set; }

        public string FabricObjects { get; set; }

        //[RequiredEx]
        public IFormFile FilePDF { get; set; }

        //[RequiredEx]
        public IFormFile FileJSON { get; set; }
    }
}
