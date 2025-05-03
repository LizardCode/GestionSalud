using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.Framework.Application.Interfaces.Context;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class AsientosAperturaRepository : BaseRepository, IAsientosAperturaRepository
    {
        public AsientosAperturaRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var builder = _context.Connection.QueryBuilder($@"
                    SELECT 
                        a.*,
                        ta.Descripcion TipoAsiento,
                        (e.Codigo + ' - ' + FORMAT(e.FechaInicio, 'dd/MM/yyyy') + ' a ' + FORMAT(e.FechaFin, 'dd/MM/yyyy')) Ejercicio
                    FROM AsientosAperturaCierre a 
                        INNER JOIN TipoAsientosAuto ta ON a.IdTipoAsientoAuto = ta.IdTipoAsientoAuto
                        INNER JOIN Ejercicios e ON a.IdEjercicio = e.IdEjercicio");

            return base.GetAllCustomQuery(builder);
        }

        public async Task<Custom.AsientoAperturaCierre> GetByIdCustom(int idAsientoAperturaCierre, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT 
                    a.*,
                    ta.Descripcion TipoAsiento,
                    (e.Codigo + ' - ' + FORMAT(e.FechaInicio, 'dd/MM/yyyy') + ' a ' + FORMAT(e.FechaFin, 'dd/MM/yyyy')) Ejercicio
                    FROM AsientosAperturaCierre a 
                        INNER JOIN TipoAsientosAuto ta ON a.IdTipoAsientoAuto = ta.IdTipoAsientoAuto
                        INNER JOIN Ejercicios e ON a.IdEjercicio = e.IdEjercicio
                    WHERE
                        a.IdAsientoAperturaCierre = {idAsientoAperturaCierre}");

            return await builder.QuerySingleAsync<Custom.AsientoAperturaCierre>(transaction);
        }
    }
}
