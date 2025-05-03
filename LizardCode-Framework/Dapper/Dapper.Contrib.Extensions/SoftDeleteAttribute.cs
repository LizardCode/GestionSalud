using System;

namespace Dapper.Contrib.Extensions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SoftDeleteAttribute : Attribute
    {
        public object Value { get; private set; }


        public SoftDeleteAttribute(object value)
        {
            Value = value;
        }
    }
}
