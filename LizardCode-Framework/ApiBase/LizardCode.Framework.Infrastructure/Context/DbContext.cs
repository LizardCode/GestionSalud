using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;


namespace LizardCode.Framework.Infrastructure.Context
{
    public class DbContext : IDbContext
    {
        private readonly Guid _id;
        private readonly IDbConnectionFactory _connFactory;

        public Guid Id => _id;
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; set; }


        public DbContext(IDbConnectionFactory connFactory)
        {
            _id = new Guid();
            _connFactory = connFactory;
            Connection = connFactory.Create();
        }


        public void Dispose() => Connection?.Dispose();
    }
}
