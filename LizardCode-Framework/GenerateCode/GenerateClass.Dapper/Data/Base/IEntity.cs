namespace GenerateClass.Dapper.Data
{
    public interface IEntity<ID>
    {
        ID Id { get; set; }
    }
}
