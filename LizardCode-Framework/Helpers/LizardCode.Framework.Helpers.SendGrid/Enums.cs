using System.ComponentModel;

namespace LizardCode.Framework.Helpers.SendGrid
{
    public class Enums
    {
        public enum StatusEmailCode
        {
            [Description("not_delivered")]
            not_delivered,
            [Description("delivered")]
            delivered
        }
    }
}
