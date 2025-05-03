using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class EjerciciosRepository : BaseRepository, IEjerciciosRepository
    {
        public EjerciciosRepository(IDbContext context) : base(context)
        {

        }

        public async Task<List<Ejercicio>> GetAllByIdEmpresa(int idEmpresa)
        {
            var builder = _context.Connection
                .FluentQueryBuilder()
                .Select($"*")
                .From($"Ejercicios")
                .Where($"IdEmpresa = {idEmpresa}")
                .OrderBy($"IdEjercicio DESC");


            var result = await builder.QueryAsync<Ejercicio>();

            return result.AsList();
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT * FROM Ejercicios");

            return base.GetAllCustomQuery(query);
        }

        public async Task<string> GetLastCodigoByIdEmpresa(int idEmpresa)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"*")
                .From($"Ejercicios")
                .Where($"IdEmpresa = {idEmpresa}")
                .OrderBy($"IdEjercicio DESC");
                

            var ejercicio = await query.QueryFirstOrDefaultAsync<Ejercicio>();

            return ejercicio?.Codigo ?? default;

        }

        public async Task<bool> ValidateFechaEjercicio(int idEjercicio, DateTime fecha, int idEmpresa)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"*")
                .From($"Ejercicios")
                .Where($" IdEjercicio = {idEjercicio}")
                .Where($" {fecha} BETWEEN fechaInicio AND fechaFin")
                .Where($" IdEmpresa = {idEmpresa}");

                var ejercicio = await query.QueryFirstOrDefaultAsync<Ejercicio>();

            return ejercicio == null;

        }

        public async Task<bool> EjercicioCerrado(int idEjercicio, int idEmpresa)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"*")
                .From($"Ejercicios")
                .Where($" IdEjercicio = {idEjercicio}")
                .Where($" IdEmpresa = {idEmpresa}");

            var ejercicio = await query.QueryFirstOrDefaultAsync<Ejercicio>();

            return ejercicio?.Cerrado == Commons.Si.Description();
        }

        public async Task<bool> ValidateFechaEnOtroEjercicio(DateTime fecha, int idEmpresa)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"*")
                .From($"Ejercicios")
                .Where($" {fecha} BETWEEN fechaInicio AND fechaFin")
                .Where($" IdEmpresa = {idEmpresa}");

            var ejercicio = await query.QueryFirstOrDefaultAsync<Ejercicio>();

            return ejercicio != null;
        }

        public async Task<Ejercicio> GetCurrentByFechaIdEmpresa(DateTime fecha, int idEmpresa, IDbTransaction tran = null)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"*")
                .From($"Ejercicios")
                .Where($" {fecha} BETWEEN fechaInicio AND fechaFin")
                .Where($" IdEmpresa = {idEmpresa}");

            var ejercicio = await query.QueryFirstOrDefaultAsync<Ejercicio>();

            return ejercicio;
        }
    }
}
