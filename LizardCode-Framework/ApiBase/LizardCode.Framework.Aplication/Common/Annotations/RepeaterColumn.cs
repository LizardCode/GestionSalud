using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.Framework.Application.Common.Annotations
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class RepeaterColumnAttribute : Attribute
	{
		public string Header { get; init; }
		public int Position { get; init; }
		public int Width { get; init; }
		public bool Readonly { get; init; }
		public bool Hidden { get; init; }
        public bool Remote { get; init; }
        public string Action { get; init; }
        public string PropertyId { get; init; }
        public string PropertyText { get; init; }

        public RepeaterColumnType ControlType { get; set; }
		public RepeaterColumnAlign Align { get; set; }

		public RepeaterColumnAttribute(RepeaterColumnType controlType = RepeaterColumnType.Input, RepeaterColumnAlign align = RepeaterColumnAlign.None, bool remote = false, string action = "", string propertyId = "", string propertyText = "")
		{
			ControlType = controlType;
			Align = align;
			Remote = remote;
			Action = action;
			PropertyId = propertyId;
            PropertyText = propertyText;

        }
	}
}
