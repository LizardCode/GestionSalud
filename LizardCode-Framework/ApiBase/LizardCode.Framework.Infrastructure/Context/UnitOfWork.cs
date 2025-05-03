using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;

namespace LizardCode.Framework.Infrastructure.Context
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContext _context;


        public UnitOfWork(IDbContext context)
        {
            _context = context;
        }


        public IDbTransaction BeginTransaction()
        {
            if (_context.Connection.State != ConnectionState.Open)
            {
                _context.Connection.Close();
                _context.Connection.Open();
            }

            _context.Transaction = _context.Connection.BeginTransaction();
            return _context.Transaction;
        }

        public void Commit()
        {
            _context.Transaction.Commit();
            Dispose();
        }

        public void Rollback()
        {
            _context.Transaction.Rollback();
            Dispose();
        }

        public void Dispose() => _context.Transaction?.Dispose();
    }
}
