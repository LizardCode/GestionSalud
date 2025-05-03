using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ArticulosMovimientosStockRepository : BaseRepository, IArticulosMovimientosStockRepository
    {
        public ArticulosMovimientosStockRepository(IDbContext context) : base(context)
        {

        }
    }
}
