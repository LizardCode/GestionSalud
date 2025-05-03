using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class MenuRepository : BaseRepository, IMenuRepository
    {
        public MenuRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<Custom.Menu>> GetAllCustom()
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
						m.IdMenu,
                        m.Descripcion,
                        m.Icono,
						sm.IdSubmenu,
                        sm.Descripcion AS DescripcionSubmenu,
                        sm.Accion
                    FROM Menu m
					INNER JOIN Submenu sm
						ON m.IdMenu = sm.IdMenu
                    ORDER BY m.Orden, sm.Orden
                ");

            var results = await builder.QueryAsync<Custom.Menu>();

            return results.AsList();
        }

        public async Task<IList<Custom.Menu>> GetByTipoUsuarioCustom(int idTipoUsuario)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
						m.IdMenu,
                        m.Descripcion,
                        m.Icono,
						sm.IdSubmenu,
                        sm.Descripcion AS DescripcionSubmenu,
                        sm.Accion,
                        sm.EsReporte
                    FROM Menu m
					INNER JOIN Submenu sm ON m.IdMenu = sm.IdMenu
                    INNER JOIN TipoUsuarioSubmenu tusm ON tusm.IdSubMenu = sm.IdSubMenu
                    WHERE tusm.idTipoUsuario = {idTipoUsuario} 
                        AND sm.NoMostrar = 0
                    ORDER BY m.Orden, sm.Orden
                ");

            var results = await builder.QueryAsync<Custom.Menu>();

            return results.AsList();
        }

        public async Task<Menu> GetById<Menu>(int id)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
						m.IdMenu,
                        m.Descripcion,
                        m.Icono,
						sm.IdSubmenu,
                        sm.Descripcion AS DescripcionSubmenu,
                        sm.Accion
                    FROM Menu m
					INNER JOIN Submenu sm
						ON m.IdMenu = sm.IdMenu
                    WHERE
                        m.IdMenu = { id }
                ");

            var results = await builder.QueryFirstAsync<Menu>();

            return results;
        }

    }
}
