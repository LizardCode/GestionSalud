namespace LizardCode.Framework.Application.Interfaces.Annotations
{
    public abstract class PropertyConstraint : Attribute
    {
        public abstract Dictionary<string, string> HtmlAttributes { get; }
    }
}
