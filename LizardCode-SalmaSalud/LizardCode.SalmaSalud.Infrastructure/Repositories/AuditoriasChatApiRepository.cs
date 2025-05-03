using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using NPOI.XWPF.UserModel;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class AuditoriasChatApiRepository : BaseRepository, IAuditoriasChatApiRepository
    {
        public AuditoriasChatApiRepository(IDbContext context) : base(context)
        {

        }
        public DataTablesCustomQuery GetAllCustomQuery(int limit = 0)
        {
            var sql = limit > 0 ? BuildCustomQuery(limit) : BuildCustomQuery();
            var builder = _context.Connection.QueryBuilder(sql);

            return base.GetAllCustomQuery(builder);
        }


        private FormattableString BuildCustomQuery() =>
                $@"SELECT aca.*,
		                    eac.Descripcion as Evento,
		                    eac.Clase as EventoClase,
		                    eaca.Descripcion as Estado,
		                    eaca.Clase as EstadoClase,
		                    p.Nombre as Paciente
                    FROM AuditoriasChatApi aca
                    INNER JOIN EventoAuditoriaChatApi eac ON (eac.IdEvento = aca.IdEvento)
                    INNER JOIN EstadoAuditoriaChatApi eaca ON (eaca.IdEstadoAuditoriaChatApi = aca.IdEstadoAuditoriaChatApi)
                    INNER JOIN Pacientes p ON (p.IdPaciente = aca.IdPaciente) ";

        private FormattableString BuildCustomQuery(int limit)
        {
            var s = "SELECT TOP " + limit.ToString() + " ";

            s += $@"aca.*,
		                eac.Descripcion as Evento,
		                eac.Clase as EventoClase,
		                eaca.Descripcion as Estado,
		                eaca.Clase as EstadoClase,
		                p.Nombre as Paciente
                FROM AuditoriasChatApi aca
                INNER JOIN EventoAuditoriaChatApi eac ON (eac.IdEvento = aca.IdEvento)
                INNER JOIN EstadoAuditoriaChatApi eaca ON (eaca.IdEstadoAuditoriaChatApi = aca.IdEstadoAuditoriaChatApi)
                INNER JOIN Pacientes p ON (p.IdPaciente = aca.IdPaciente) ";

            return FormattableStringFactory.Create(s);
        }

        public async Task<Custom.AuditoriaChatApi> GetByIdCustom(int idAuditoria, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT aca.*,
		                    eac.Descripcion as Evento,
		                    eac.Clase as EventoClase,
		                                eaca.Descripcion as Estado,
		                                eaca.Clase as EstadoClase,
		                                p.Nombre as Paciente
                                FROM AuditoriasChatApi aca
                                INNER JOIN EventoAuditoriaChatApi eac ON (eac.IdEvento = aca.IdEvento)
                                INNER JOIN EstadoAuditoriaChatApi eaca ON (eaca.IdEstadoAuditoriaChatApi = aca.IdEstadoAuditoriaChatApi) 
                                INNER JOIN Pacientes p ON (p.IdPaciente = aca.IdPaciente) 
                                WHERE 
                                    aca.IdAuditoria = {idAuditoria}");

            return await builder.QuerySingleAsync<Custom.AuditoriaChatApi>(transaction);
        }

        public async Task<Custom.AuditoriaChatApiTotales> GetTotalesByEstado(int idEmpresa)
        {
            FormattableString sql = $@"SELECT SUM(CASE WHEN IdEstadoAuditoriaChatApi = {(int)EstadoAuditoriaChatApi.Error} AND YEAR(Fecha) = YEAR(getdate()) AND MONTH(Fecha) = MONTH(getdate()) AND DAY(Fecha) = DAY(getdate()) THEN 1 ELSE 0 END) as AuditoriaChatApiErrorHoy,
		                                    SUM(CASE WHEN IdEstadoAuditoriaChatApi = {(int)EstadoAuditoriaChatApi.Enviado} AND YEAR(Fecha) = YEAR(getdate()) AND MONTH(Fecha) = MONTH(getdate()) AND DAY(Fecha) = DAY(getdate()) THEN 1 ELSE 0 END) as AuditoriaChatApiEnviadosHoy
                                    FROM AuditoriasChatApi aca
                                    WHERE aca.IdEmpresa = {idEmpresa} ";

            var builder = _context.Connection
                .QueryBuilder(sql);
            //.Where($" t.IdEstadoTramite != {(int)EstadoTramite.Anulado} ");

            return await builder.QuerySingleAsync<Custom.AuditoriaChatApiTotales>();
        }
    }
}
