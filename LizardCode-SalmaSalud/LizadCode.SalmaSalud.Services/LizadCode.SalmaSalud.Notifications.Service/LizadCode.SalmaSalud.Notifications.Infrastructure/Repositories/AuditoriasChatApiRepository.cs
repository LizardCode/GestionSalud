using Dawa.Framework.Application.Interfaces.Context;
using LizadCode.SalmaSalud.Notifications.Application.Interfaces.Repositories;

namespace LizadCode.SalmaSalud.Notifications.Infrastructure.Repositories
{
    public class AuditoriasChatApiRepository : BaseRepository, IAuditoriasChatApiRepository
    {
        public AuditoriasChatApiRepository(IDbContext context) : base(context)
        {

        }
    }
}
