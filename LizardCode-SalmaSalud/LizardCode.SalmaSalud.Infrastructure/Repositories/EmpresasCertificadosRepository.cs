using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Threading.Tasks;
using System.Data;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class EmpresasCertificadosRepository : BaseRepository, IEmpresasCertificadosRepository
    {
        public EmpresasCertificadosRepository(IDbContext context) : base(context)
        {

        }

        public async Task<EmpresaCertificado> GetValidEmpresaCerificadoByIdEmpresa(int idEmpresa, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						ec.*
                    FROM EmpresasCertificados ec
                ")
                .Where($"ec.IdEmpresa = {idEmpresa}")
                .Where($"ec.Vencimiento >= {DateTime.Now}");

            return await query.QueryFirstOrDefaultAsync<EmpresaCertificado>(transaction);

        }

    }
}
