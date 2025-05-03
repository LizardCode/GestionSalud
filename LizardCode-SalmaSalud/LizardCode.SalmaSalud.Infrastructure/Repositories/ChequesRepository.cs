using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class ChequesRepository : BaseRepository, IChequesRepository
    {
        public ChequesRepository(IDbContext context) : base(context)
        {

        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT 
                                        c.*, 
                                        ec.Descripcion AS EstadoCheque, 
                                        tc.Descripcion AS TipoCheque,
                                        cda.IdAsiento
                                FROM Cheques c
                                    INNER JOIN estadoscheques ec ON c.IdEstadoCheque = ec.idEstadoCheque
                                    INNER JOIN tipocheques tc ON c.IdTipoCheque = tc.IdTipoCheque
                                    LEFT JOIN cheques_debitos_asientos cda ON c.IdCheque = cda.IdCheque");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Cheque> GetByNumeroCheque(string numeroCheque, int idBanco, int idTipoCheque, int idEmpresa, IDbTransaction tran = null)
        {
            var builder = _context.Connection
               .QueryBuilder($"SELECT * FROM Cheques ")
               .Where($"IdBanco = {idBanco}")
               .Where($"IdEmpresa = {idEmpresa}")
               .Where($"IdTipoCheque = {idTipoCheque}")
               .Where($"IdEstadoRegistro <> {(int)Domain.Enums.EstadoRegistro.Eliminado}")
               .Where($"NroCheque = {numeroCheque}");

            return await builder.QueryFirstOrDefaultAsync<Cheque>(tran);
        }

        public async Task<List<Domain.EntitiesCustom.ChequeTerceros>> GetChequesCartera(int idEmpresa, string term)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"ch.IdCheque Id")
                .Select($"(ch.NroCheque + ' - ' + ch.Banco) Text")
                .Select($"ch.Banco")
                .Select($"ch.NroCheque")
                .Select($"ch.FechaEmision Fecha")
                .Select($"ch.FechaVto")
                .Select($"ch.Importe")
                .From($"Cheques ch")
                .Where($"ch.IdEstadoRegistro <> {(int)Domain.Enums.EstadoRegistro.Eliminado}")
                .Where($"ch.IdEstadoCheque = {(int)Domain.Enums.EstadoCheque.EnCartera}")
                .Where($"ch.IdTipoCheque = {(int)Domain.Enums.TipoCheque.Terceros}")
                .Where($"(ch.NroCheque LIKE {string.Concat("%", term, "%")} OR ch.Banco LIKE {string.Concat("%", term, "%")})")
                .Where($"ch.IdEmpresa = {idEmpresa}");


            var result = await query.QueryAsync<Domain.EntitiesCustom.ChequeTerceros>();

            return result.AsList();
        }

        public async Task<Cheque> GetPrimerChequeDisponible(int idBanco, int idTipoCheque, int idEmpresa)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"ch.IdCheque Id")
                .Select($"ch.NroCheque")
                .From($"Cheques ch")
                .Where($"ch.IdBanco = {idBanco}")
                .Where($"ch.IdEstadoRegistro <> {(int)Domain.Enums.EstadoRegistro.Eliminado}")
                .Where($"ch.IdTipoCheque = {idTipoCheque}")
                .Where($"ch.IdEstadoCheque = {Domain.Enums.EstadoCheque.SinLibrar}")
                .Where($"ch.IdEmpresa = {idEmpresa}");

            return await query.QueryFirstOrDefaultAsync<Cheque>();
        }

        public async Task<bool> ValidarNumeroChequeDisponible(int idBanco, int idTipoCheque, string nroCheque, int idEmpresa)
        {
            var query = _context.Connection
                .FluentQueryBuilder()
                .Select($"ch.*")
                .From($"Cheques ch")
                .Where($"ch.IdBanco = {idBanco}")
                .Where($"ch.IdEstadoRegistro <> {(int)Domain.Enums.EstadoRegistro.Eliminado}")
                .Where($"ch.IdTipoCheque = {idTipoCheque}")
                .Where($"ch.NroCheque = {nroCheque}")
                .Where($"ch.IdEstadoCheque = {Domain.Enums.EstadoCheque.SinLibrar}")
                .Where($"ch.IdEmpresa = {idEmpresa}");

            var result = await query.QueryFirstOrDefaultAsync<Cheque>();

            return result != null;

        }

        public async Task<bool> ValidarNumeroCheque(int idBanco, int idTipoCheque, string nroDesde, string nroHasta)
        {
            var builder = _context.Connection
                .QueryBuilder($"SELECT * FROM Cheques ")
                .Where($"IdBanco = {idBanco}")
                .Where($"IdEstadoRegistro <> {(int)Domain.Enums.EstadoRegistro.Eliminado}")
                .Where($"IdTipoCheque = {idTipoCheque}");

            if (!nroDesde.IsNull())
            {
                builder.Where($"NroCheque >= '{nroDesde.PadLeft(10, '0')}'");
            }

            if (!nroHasta.IsNull())
            {
                builder.Where($"NroCheque <= '{nroHasta.PadLeft(10, '0')}'");
            }

            var result = await builder.QueryAsync<Cheque>();

            return result.AsList().Count == 0;
        }

        public async Task<List<Domain.EntitiesCustom.Cheque>> GetChequesADebitar(int idEmpresa, int idBanco)
        {
            var query = _context.Connection
                .QueryBuilder($"SELECT * FROM Cheques ")
                .Where($"IdBanco = {idBanco}")
                .Where($"IdEmpresa = {idEmpresa}")
                .Where($"IdTipoCheque IN ({(int)TipoCheque.E_ChequeDiferido}, {(int)TipoCheque.Diferido }, {(int)TipoCheque.E_ChequeComun}, {(int)TipoCheque.Comun}) ")
                .Where($"IdEstadoCheque IN ({(int)EstadoCheque.Entregado}, {(int)EstadoCheque.Librado})")
                .Where($"IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}");

            var result = await query.QueryAsync<Domain.EntitiesCustom.Cheque>();

            return result.AsList();
        }

        public async Task<bool> DeleteByIdCheque(int idCheque, IDbTransaction transaction = null)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    DELETE FROM Cheques
                    WHERE 
                        IdCheque = {idCheque} ");

            var results = await builder.ExecuteAsync(transaction);

            return (results == 1);
        }

        public async Task<bool> GetVerificarDuplicados(int idEmpresa, int idBanco, int idTipoCheque, string nroDesde, string nroHasta)
        {
            var builder = _context.Connection
                .CommandBuilder($@"
                    SELECT * FROM Cheques
                    WHERE 
                        IdEmpresa = {idEmpresa} AND 
                        IdBanco = {idBanco} AND
                        IdTipoCheque = {idTipoCheque} AND
                        IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                        NroCheque >= {nroDesde.PadLeft(10, '0')} AND
                        NroCheque <= {nroHasta.PadLeft(10, '0')}");

            var results = await builder.QueryAsync();

            return results.AsList().Count != 0;
        }
    }
}
