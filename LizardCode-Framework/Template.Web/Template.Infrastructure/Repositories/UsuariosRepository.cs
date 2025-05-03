using LizardCode.Framework.Infrastructure.Interfaces.Context;
using LizardCode.Framework.Infrastructure.Repositories;
using Template.Application.Interfaces.Repositories;

namespace Template.Infrastructure.Repositories
{
    public class UsuariosRepository : BaseRepository, IUsuariosRepository
    {
        public UsuariosRepository(IDbContext context) : base(context)
        {
        }
    }
}
