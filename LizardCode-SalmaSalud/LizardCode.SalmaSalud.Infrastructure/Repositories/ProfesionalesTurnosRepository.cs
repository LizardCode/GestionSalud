using Dapper;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Linq;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ProfesionalesTurnosRepository : BaseRepository, IProfesionalesTurnosRepository
    {
        public ProfesionalesTurnosRepository(IDbContext context) : base(context)
        {
        }

        public async Task<IList<ProfesionalTurno>> GetAllByIdProfesionalAndIdEmpresa(DateTime desde, DateTime hasta, int idProfesional, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ce.*
                    FROM ProfesionalesTurnos ce
                    WHERE
                        ce.IdProfesional = { idProfesional }
                        AND ce.IdEmpresa = { idEmpresa } 
                        AND ce.fechaInicio >= { desde }  
                        AND ce.fechaFin <= { hasta } ");

            var results = await builder.QueryAsync<ProfesionalTurno>(transaction);

            return results.AsList();
        }

        public async Task<IList<ProfesionalTurno>> GetByIdProfesionalAndIdEmpresaAndFecha(DateTime desde, int idProfesional, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT
	                    ce.*
                    FROM ProfesionalesTurnos ce
                    WHERE
                        ce.IdProfesional = {idProfesional}
                        AND ce.IdEmpresa = {idEmpresa} 
                        AND ce.fechaInicio = {desde} ");

            var results = await builder.QueryAsync<ProfesionalTurno>(transaction);

            return results.AsList();
        }

        public async Task<bool> RemoveByIdProfesional(int idProfesional, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE ce FROM ProfesionalesTurnos ce                        
                    WHERE 
                        ce.IdProfesional = {idProfesional}");

            var results = await builder.ExecuteAsync(transaction);

            return (results > 1);
        }
        public async Task<bool> RemoveById(int idProfesionalTurno, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE ce FROM ProfesionalesTurnos ce                        
                    WHERE 
                        ce.idProfesionalTurno = {idProfesionalTurno}");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }
        public async Task<bool> RemoveAllByFecha(DateTime desde, DateTime hasta, int idProfesional, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"                    
                    DELETE ce FROM ProfesionalesTurnos ce  
                    WHERE
                        ce.IdProfesional = {idProfesional}
                        AND ce.IdEmpresa = {idEmpresa} 
                        AND ce.fechaInicio >= {desde}  
                        AND ce.fechaFin <= {hasta} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results > 0);
        }

        public async Task<bool> ExistenTurnosAgendadosByFecha(DateTime desde, DateTime hasta, int idProfesional, int idEmpresa, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .QueryBuilder($@"                    
                    SELECT ISNULL(COUNT(*) , 0)
                    FROM ProfesionalesTurnos ce  
                    WHERE
                        ce.IdEstadoProfesionalTurno = {(int)EstadoProfesionalTurno.Agendado}
                        AND ce.IdProfesional = {idProfesional}
                        AND ce.IdEmpresa = {idEmpresa} 
                        AND ce.fechaInicio >= {desde}  
                        AND ce.fechaFin <= {hasta} ");

            //var results = await builder.ExecuteAsync(transaction);
            var results = await builder.ExecuteScalarAsync<int>(transaction);

            return (results > 0);
        }
    }
}
