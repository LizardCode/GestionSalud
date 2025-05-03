using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class DepositosBancoRepository : BaseRepository, IDepositosBancoRepository
    {
        public DepositosBancoRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT db.*, b.Descripcion Banco FROM DepositosBanco db INNER JOIN Bancos b ON db.IdBanco = b.IdBanco");

            return base.GetAllCustomQuery(query);
        }

        public async Task<List<dynamic>> GetDepositoByCheque(int idCheque)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
		                db.IdDepositoBanco, 
                        b.Descripcion Banco, 
                        b.CUIT,
                        b.NroCuenta
                    FROM DepositosBancoDetalle dbd
                        INNER JOIN DepositosBanco db ON dbd.IdDepositoBanco = db.IdDepositoBanco
                        INNER JOIN Bancos b ON db.IdBanco = b.IdBanco
                    ");

            builder.Where($"db.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");
            builder.Where($"dbd.IdCheque = {idCheque}");

            return (await builder.QueryAsync<dynamic>()).AsList();
        }
    }
}
