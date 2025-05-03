
using System.Data;

namespace LizadCode.SalmaSalud.Notifications.Application.Interfaces.Repositories
{
    public interface IAuditoriasChatApiRepository
    {
        Task<long> Insert<TAuditoria>(TAuditoria entity, IDbTransaction transaction = null);
    }
}
