using System.Data;

namespace LizardCode.Framework.Application.Interfaces.Context
{
    public interface IUnitOfWork : IDisposable
    {
        IDbTransaction BeginTransaction();
        void Commit();
        void Rollback();
    }
}
