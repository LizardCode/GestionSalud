using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LizardCode.Framework.Application.Models.Repeater
{
	public class RepeaterColumn
	{
		public RepeaterColumnType Type { get; set; }
		public string PropertyName { get; }
		public SelectList ValueList { get; set; }
		public string Header { get; }
		public int Position { get; }
		public int Width { get; }
		public bool Readonly { get; }
        public bool Remote { get; init; }
        public string Action { get; init; }
        public string PropertyId { get; init; }
        public string PropertyText { get; init; }

        public RepeaterColumnAlign Align { get; set; }


		public RepeaterColumn(
			RepeaterColumnType type,
			string propertyName,
			SelectList valueList,
			string header,
			int position,
			int width = 0,
			RepeaterColumnAlign align = RepeaterColumnAlign.Left,
            bool remote = false,
			string action = "",
            string propertyId = "",
            string propertyText = "",
            bool @readonly = false)
		{
			Type = type;
			PropertyName = propertyName;
			ValueList = valueList;
			Header = header;
			Position = position;
			Width = width;
			Align = align;
			Remote = remote;
			Action = action;
			PropertyId = propertyId;
            PropertyText = propertyText;
            Readonly = @readonly;
		}
	}
}
