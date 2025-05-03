using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.Framework.Application.Models.MasterDetail
{
    public class MasterDetailData
    {
        public string PropertyName { get; }
        public SelectList Values { get; }
        public string PropertyDisplayName { get; }

        public MasterDetailData(string propertyName, SelectList values, string propertyDisplayName)
        {
            PropertyName = propertyName;
            Values = values;
            PropertyDisplayName = propertyDisplayName;
        }
    }
}
