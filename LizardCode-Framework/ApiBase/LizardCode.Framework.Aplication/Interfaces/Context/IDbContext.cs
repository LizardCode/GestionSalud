using System.Data;

namespace LizardCode.Framework.Application.Interfaces.Context
{
    public interface IDbContext : IDisposable
    {
        public Guid Id { get; }
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; set; }
    }
}

