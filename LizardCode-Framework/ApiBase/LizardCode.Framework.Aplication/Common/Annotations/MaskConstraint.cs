using LizardCode.Framework.Application.Common.Annotations.Base;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.Framework.Application.Common.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MaskConstraintAttribute : PropertyConstraint
    {
        private readonly Dictionary<string, string> _attributes;

        public MaskConstraintType Type { get; private set; }
        public string Delimiters { get; private set; }
        public string Blocks { get; private set; }
        public string DatePattern { get; private set; }
        public bool NumericOnly { get; private set; }
        public bool Uppercase { get; private set; }

        public override Dictionary<string, string> HtmlAttributes => _attributes;

        public MaskConstraintAttribute(MaskConstraintType type, string delimiters = "", string blocks = "", string datePattern = "", bool numericOnly = false, bool uppercase = false)
        {
            _attributes = new Dictionary<string, string>();

            Type = type;
            Delimiters = delimiters;
            Blocks = blocks;
            DatePattern = datePattern;
            NumericOnly = numericOnly;
            Uppercase = uppercase;

            Validate();
            Init();

        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Blocks) && (Type == MaskConstraintType.String || Type == MaskConstraintType.Custom))
                throw new ArgumentNullException(nameof(Blocks));

            if (string.IsNullOrWhiteSpace(DatePattern) && Type == MaskConstraintType.Date)
                throw new ArgumentNullException(nameof(DatePattern));
        }

        private void Init()
        {
            _attributes.Add("data-constraint", "mask");
            _attributes.Add("data-constraint-type", Type.ToString().ToLower());
            _attributes.Add("data-constraint-blocks", Blocks.ToString());
            _attributes.Add("data-constraint-datepattern", DatePattern.ToString());
            _attributes.Add("data-constraint-delimiters", Delimiters.ToString());
            _attributes.Add("data-constraint-numericOnly", NumericOnly.ToString().ToLower());
            _attributes.Add("data-constraint-uppercase", Uppercase.ToString().ToLower());
        }
    }
}
