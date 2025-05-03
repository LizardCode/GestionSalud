namespace LizardCode.Framework.Application.Common.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MasterDetailColumnAttribute : Attribute
    {
        public string Header { get; init; }
        public int Position { get; init; }
        public int Width { get; init; }
        public bool Hidden { get; init; }
    }
}
