using Newtonsoft.Json;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Models.Plantillas
{
    public class FabricRoot
    {
        [JsonProperty("objects")]
        public List<TagMapping> TagsMapping { get; set; }
        [JsonProperty("backgroundImage")]
        public BackgroundImage Image { get; set; }
    }
}
