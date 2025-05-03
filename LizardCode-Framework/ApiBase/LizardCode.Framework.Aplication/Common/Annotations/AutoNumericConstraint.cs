using LizardCode.Framework.Application.Common.Annotations.Base;
using LizardCode.Framework.Application.Common.Enums;

namespace LizardCode.Framework.Application.Common.Annotations
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class AutoNumericConstraintAttribute : PropertyConstraint
    {
        private readonly AutoNumericConstraintType _type;
        private readonly string _currencySymbol;
        private readonly string _decimalCharacter;
        private readonly string _digitGroupSeparator;
        private readonly int _decimalPlaces;

        private readonly double _maximumValue;
        private readonly double _minimumValue;

        private readonly Dictionary<string, string> _attributes;

        public AutoNumericConstraintType Type { get { return _type; } }
        public string CurrencySymbol { get { return _currencySymbol; } }
        public string DecimalCharacter { get { return _decimalCharacter; } }
        public string DigitGroupSeparator { get { return _digitGroupSeparator; } }
        public int DecimalPlaces { get { return _decimalPlaces; } }

        public double MaximumValue { get { return _maximumValue; } }
        public double MinimumValue { get { return _minimumValue; } }

        public override Dictionary<string, string> HtmlAttributes => _attributes;


        public AutoNumericConstraintAttribute(AutoNumericConstraintType type, string currencySymbol = "", string decimalCharacter = ",", string digitGroupSeparator = ".", int decimalPlaces = 2, double minimumValue = -10000000000000, double maximumValue = 10000000000000)
        {
            _type = type;
            _attributes = new Dictionary<string, string>();

            _currencySymbol = currencySymbol;
            _decimalCharacter = decimalCharacter;
            _digitGroupSeparator = digitGroupSeparator;
            _decimalPlaces = decimalPlaces;
            _minimumValue = minimumValue;
            _maximumValue = maximumValue;

            Validate();
            Init();
        }

        private void Validate()
        {
            switch (_type)
            {
                case AutoNumericConstraintType.Numeric:
                    if (string.IsNullOrWhiteSpace(_decimalCharacter))
                        throw new ArgumentNullException(nameof(_decimalCharacter));
                    break;
                case AutoNumericConstraintType.Currency:
                    if (string.IsNullOrWhiteSpace(_decimalCharacter))
                        throw new ArgumentNullException(nameof(_decimalCharacter));
                    if (string.IsNullOrWhiteSpace(_digitGroupSeparator))
                        throw new ArgumentNullException(nameof(_digitGroupSeparator));
                    break;
                case AutoNumericConstraintType.Percentage:
                    if (string.IsNullOrWhiteSpace(_decimalCharacter))
                        throw new ArgumentNullException(nameof(_decimalCharacter));
                    break;
            }
        }

        private void Init()
        {
            _attributes.Add("data-constraint", "autonumeric");
            _attributes.Add("data-constraint-type", _type.ToString().ToLower());
            switch (_type)
            {
                case AutoNumericConstraintType.Numeric:
                    _attributes.Add("data-constraint-decimalcharacter", _decimalCharacter.ToString().ToLower());
                    _attributes.Add("data-constraint-digitgroupseparator", _digitGroupSeparator.ToString().ToLower());
                    _attributes.Add("data-constraint-decimalplaces", _decimalPlaces.ToString().ToLower());
                    _attributes.Add("data-constraint-minimumValue", _minimumValue.ToString().ToLower());
                    _attributes.Add("data-constraint-maximumValue", _maximumValue.ToString().ToLower());
                    break;
                case AutoNumericConstraintType.Currency:
                    _attributes.Add("data-constraint-currencysymbol", _currencySymbol.ToString().ToLower());
                    _attributes.Add("data-constraint-decimalcharacter", _decimalCharacter.ToString().ToLower());
                    _attributes.Add("data-constraint-digitgroupseparator", _digitGroupSeparator.ToString().ToLower());
                    _attributes.Add("data-constraint-decimalplaces", _decimalPlaces.ToString().ToLower());
                    _attributes.Add("data-constraint-minimumValue", _minimumValue.ToString().ToLower());
                    _attributes.Add("data-constraint-maximumValue", _maximumValue.ToString().ToLower());
                    break;
                case AutoNumericConstraintType.Percentage:
                    _attributes.Add("data-constraint-decimalcharacter", _decimalCharacter.ToString().ToLower());
                    _attributes.Add("data-constraint-decimalplaces", _decimalPlaces.ToString().ToLower());
                    break;
            }

        }
    }
}
