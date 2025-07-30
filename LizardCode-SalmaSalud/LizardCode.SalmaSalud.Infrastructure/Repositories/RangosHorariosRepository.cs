using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class RangosHorariosRepository : BaseRepository, IRangosHorariosRepository
    {
        public RangosHorariosRepository(IDbContext context) : base(context)
        {
        }
    }
}