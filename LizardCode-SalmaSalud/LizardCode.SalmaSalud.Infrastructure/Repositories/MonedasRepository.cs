using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class MonedasRepository : BaseRepository, IMonedasRepository
    {
        public MonedasRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT m.* FROM Monedas m");

            return base.GetAllCustomQuery(query);
        }
        public DataTablesCustomQuery GetTiposDeCambio()
        {
            FormattableString sql = $@"SELECT m.Simbolo, m.Descripcion, mfc.fecha, mfc.Cotizacion
                                        FROM MonedasFechasCambio mfc
                                        INNER JOIN Monedas m ON (mfc.IdMoneda = m.IdMoneda)
                                        INNER JOIN (
	                                        SELECT IdMoneda, MAX(fecha) maxFecha
	                                        FROM MonedasFechasCambio
	                                        GROUP BY IdMoneda
                                        ) as max_mfc on (mfc.Fecha = max_mfc.maxFecha AND mfc.IdMoneda = max_mfc.IdMoneda) 
                                        WHERE m.idEstadoRegistro <> {(int)EstadoRegistro.Eliminado} ";

            var builder = _context.Connection.QueryBuilder(sql);

            return base.GetAllCustomQuery(builder);
        }

    }
}
