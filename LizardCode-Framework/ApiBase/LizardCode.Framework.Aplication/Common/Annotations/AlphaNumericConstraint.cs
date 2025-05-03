using LizardCode.Framework.Application.Common.Annotations.Base;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.Framework.Application.Common.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class AlphaNumericConstraintAttribute : PropertyConstraint
    {
        private readonly Dictionary<string, string> _attributes;

        public AlphaNumConstraintType Type { get; private set; }
        public string Charset { get; private set; }


        public override Dictionary<string, string> HtmlAttributes => _attributes;


        public AlphaNumericConstraintAttribute(AlphaNumConstraintType type, string charset = null)
        {
            _attributes = new Dictionary<string, string>();

            Type = type;
            Charset = charset;

            Validate();
            Init();
        }

        private void Validate()
        {
            if (Type == AlphaNumConstraintType.SpecificCharset && string.IsNullOrWhiteSpace(Charset))
                throw new ArgumentNullException(nameof(Charset));
        }

        private void Init()
        {
            _attributes.Add("data-constraint", "alphanum");
            _attributes.Add("data-constraint-type", Type.ToString().ToLower());

            if (Type == AlphaNumConstraintType.SpecificCharset)
                _attributes.Add("data-constraint-charset", Charset);
        }
    }
}
