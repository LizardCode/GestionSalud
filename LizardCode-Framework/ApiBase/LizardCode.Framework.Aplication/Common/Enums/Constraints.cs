namespace LizardCode.Framework.Application.Common.Enums
{
    public enum AlphaNumConstraintType
    {
        Alpha,
        AlphaNumeric,
        AlphaPlusCharset,
        AlphaNumericPlusCharset,
        SpecificCharset
    }

    public enum MaskConstraintType
    {
        RegExp,
        String,
        Number,
        Date,
        Custom
    }

    public enum AutoNumericConstraintType
    {
        Numeric,
        Currency,
        Percentage
    }
}
