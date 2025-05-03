using System;

namespace LizardCode.Framework.Helpers.Utilities.Attributes
{
    public class CodeAttribute : Attribute
    {
        public string Value { get; private set; }

        public CodeAttribute(string value)
        {
            Value = value;
        }
    }
}
