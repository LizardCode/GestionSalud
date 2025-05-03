using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class CodigosRetencionIVARepository : BaseRepository, ICodigosRetencionIVARepository
    {
        public CodigosRetencionIVARepository(IDbContext context) : base(context)
        {
        }

        public async Task<bool> DeleteByIdCodigoRetencion(int idCodigoRetencion, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM CodigosRetencionIVA
                    WHERE IdCodigoRetencion = {idCodigoRetencion}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}
