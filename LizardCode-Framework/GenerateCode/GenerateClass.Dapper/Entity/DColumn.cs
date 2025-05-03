namespace GenerateClass.Dapper
{
    public class DColumn
    {
        public virtual string ColumnName { get; set; }
        public virtual string ColumnTypeNet { get; set; }
        public virtual int MaxLengthString { get; set; }
        public virtual bool IsNullable { get; set; }
        public virtual bool IsIdentity { get; set; }
        public virtual bool IsPrimaryKey { get; set; }
        public virtual bool IsForeignKey { get; set; }
        public virtual string ReferencedTable { get; set; }
    }
}
