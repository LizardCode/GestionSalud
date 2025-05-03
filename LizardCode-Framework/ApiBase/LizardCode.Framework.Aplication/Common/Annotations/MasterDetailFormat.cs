using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.Framework.Application.Common.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MasterDetailFormatAttribute : Attribute
    {
        public MasterDetailColumnFormat Format { get; init; }
    }
}
