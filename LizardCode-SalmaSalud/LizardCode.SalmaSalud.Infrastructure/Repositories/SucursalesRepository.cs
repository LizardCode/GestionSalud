using Dapper;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.AFIP;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class SucursalesRepository : BaseRepository, ISucursalesRepository
    {
        public SucursalesRepository(IDbContext context) : base(context)
        {

        }

        public async Task<string> GetAFIPConsultaNumeracion(AfipAuth afipAuth, string cuit, int codigoSucursal, int codigo, bool useProd = false)
        {
            var urlKey = "WSFE-URL" + (useProd ? "" : "-DEV");
            var url = urlKey.FromAppSettings<string>(notFoundException: true);

            var compElectronicos = new ComprobantesElectronicos(afipAuth.Token, afipAuth.Sign, cuit);
            compElectronicos.Conectar(url);
            
            var nroComprobante = await compElectronicos.GetCompUltimoAutorizadoAsync(codigoSucursal, codigo);

            return nroComprobante.ToString().PadLeft(8, '0');
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"SELECT * FROM Sucursales");

            return base.GetAllCustomQuery(query);
        }

        public async Task<IList<Sucursal>> GetAllSucursalesByIdEmpresa(int idEmpresa)
        {
            var builder = _context.Connection
                .QueryBuilder($@"SELECT su.* FROM Sucursales su
                                    WHERE 
                                        su.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado} AND
                                        su.IdEmpresa = {idEmpresa} ");

            return (await builder.QueryAsync<Sucursal>()).AsList();
        }
    }
}
