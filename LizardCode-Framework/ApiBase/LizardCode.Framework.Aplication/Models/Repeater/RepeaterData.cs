using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.Framework.Application.Models.Repeater
{
    public class RepeaterData
    {
        public string PropertyName { get; }
        public SelectList Values { get; }


        public RepeaterData(string propertyName, SelectList values)
        {
            PropertyName = propertyName;
            Values = values;
        }
    }
}
