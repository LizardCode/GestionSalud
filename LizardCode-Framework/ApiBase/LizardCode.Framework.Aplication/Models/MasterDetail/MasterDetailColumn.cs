using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.Framework.Application.Models.MasterDetail
{
    public class MasterDetailColumn
    {
        public MasterDetailColumnType Type { get; set; }
        public MasterDetailColumnFormat Format { get; set; }
        public string PropertyName { get; }
        public SelectList ValueList { get; set; }
        public string Header { get; }
        public int Position { get; }
        public int Width { get; }
        public string PropertyDisplayName { get; }


        public MasterDetailColumn(
            MasterDetailColumnType type,
            MasterDetailColumnFormat format,
            string propertyName,
            SelectList valueList,
            string header,
            int position,
            int width = 0,
            string propertyDisplayName = null)
        {
            Type = type;
            Format = format;
            PropertyName = propertyName;
            ValueList = valueList;
            Header = header;
            Position = position;
            Width = width;
            PropertyDisplayName = propertyDisplayName;
        }
    }
}
