using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class EspecialidadesRepository : BaseRepository, IEspecialidadesRepository
    {
        public EspecialidadesRepository(IDbContext context) : base(context)
        {
        }
    }
}
