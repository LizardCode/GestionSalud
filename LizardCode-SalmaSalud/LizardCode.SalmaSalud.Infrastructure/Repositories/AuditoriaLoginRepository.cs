using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Context;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class AuditoriaLoginRepository : BaseRepository, IAuditoriaLoginRepository
    {
        public AuditoriaLoginRepository(IDbContext context) : base(context)
        {

        }
        public DataTablesCustomQuery GetAllCustomQuery(int idEmpresa)
        {
            var query = _context.Connection
                .QueryBuilder($@"
						SELECT u.Nombre Usuario, MAX(al.Fecha) as Fecha FROM AuditoriaLogin al
                            INNER JOIN Usuarios u ON (al.IdUsuario = u.IdUsuario)
                        WHERE 
                            al.idEmpresa = {idEmpresa}
                        GROUP BY u.Nombre");

            return base.GetAllCustomQuery(query);
        }
    }
}
