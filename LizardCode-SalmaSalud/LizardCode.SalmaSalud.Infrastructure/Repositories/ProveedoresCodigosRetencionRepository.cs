using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ProveedoresCodigosRetencionRepository : BaseRepository, IProveedoresCodigosRetencionRepository
    {
        public ProveedoresCodigosRetencionRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<ProveedorCodigoRetencion>> GetAllByIdProveedor(int idProveedor, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    pcr.*
                    FROM ProveedoresCodigosRetencion pcr
                    WHERE
                        pcr.IdProveedor = {idProveedor}");

            var results = await builder.QueryAsync<ProveedorCodigoRetencion>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdProveedor(int idProveedor, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM ProveedoresCodigosRetencion
                    WHERE IdProveedor = {idProveedor}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
    }
}
