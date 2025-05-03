namespace Dapper.DataTables.Internal
{
    internal class FilterCondition
    {
        public string FieldName { get; }
        public string Operation { get; }
        public string Token { get; }

        public FilterCondition(string fieldName, string operation, string token)
        {
            FieldName = fieldName;
            Operation = operation;
            Token = token;
        }
    }
}
