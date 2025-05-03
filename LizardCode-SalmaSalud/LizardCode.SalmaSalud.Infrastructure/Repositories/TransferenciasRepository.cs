using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class TransferenciasRepository : BaseRepository, ITransferenciasRepository
    {
        public TransferenciasRepository(IDbContext context) : base(context)
        {

        }

        public async Task<bool> DeleteById(int idTransferencia, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM Transferencias
                    WHERE 
                        IdTransferencia = {idTransferencia} AND
                        IdEmpresa = {idEmpresa} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}
