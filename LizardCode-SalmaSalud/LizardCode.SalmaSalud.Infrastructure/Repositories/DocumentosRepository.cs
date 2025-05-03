using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class DocumentosRepository : BaseRepository, IDocumentosRepository
    {
        public DocumentosRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteById(int idDocumento, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM Documentos
                    WHERE 
                        IdDocumento = {idDocumento} AND
                        IdEmpresa = {idEmpresa} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}
