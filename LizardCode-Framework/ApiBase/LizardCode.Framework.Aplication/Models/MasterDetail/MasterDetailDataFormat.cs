namespace LizardCode.Framework.Application.Models.MasterDetail
{
    public class MasterDetailDataFormat
    {
        public string DataAttributesFormatName { get; }
        public string DataAttributesFormatValues { get; }

        public MasterDetailDataFormat(string attributesFormatName, string attributesFormatValue)
        {
            DataAttributesFormatName = attributesFormatName;
            DataAttributesFormatValues = attributesFormatValue;
        }
    }
}
