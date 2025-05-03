using System.Collections.Generic;

namespace Dapper.DataTables.Internal
{
    internal class FilterOperation
    {
        public string Operation { get; init; }
        public IList<FilterCondition> Conditions { get; init; }

    }
}
