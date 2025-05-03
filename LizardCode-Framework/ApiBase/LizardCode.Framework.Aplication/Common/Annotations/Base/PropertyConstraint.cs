namespace LizardCode.Framework.Application.Common.Annotations.Base
{
    public abstract class PropertyConstraint : Attribute
    {
        public abstract Dictionary<string, string> HtmlAttributes { get; }
    }
}
