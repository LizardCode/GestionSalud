using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    internal class ArchivosRepository : BaseRepository, IArchivosRepository
    {
        public ArchivosRepository(IDbContext context) : base(context)
        {
        }
    }
}
