using Dapper;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.AFIP;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class AfipAuthRepository : BaseRepository, IAfipAuthRepository
    {
        public AfipAuthRepository(IDbContext context) : base(context)
        {
            //
        }

        public async Task<Domain.Entities.AfipAuth> GetValidSignToken(int idEmpresa, string servicio)
        {
            var query = _context.Connection
                .CommandBuilder($@"
                    SELECT aa.* FROM AfipAuth aa
                    WHERE 
                        aa.IdEmpresa = {idEmpresa} AND
                        aa.Servicio = {servicio} AND
                        aa.ExpirationTime >= {DateTime.Now}");

            return await query.QueryFirstOrDefaultAsync<Domain.Entities.AfipAuth>();

        }

        public async Task<Domain.Entities.AfipAuth> NewSignToken(int idEmpresa, string crt, string pk, string cuit, string servicio, bool useProd = false)
        {
            var urlKey = "WSAA-URL" + (useProd ? "" : "-DEV");
            var url = urlKey.FromAppSettings<string>(notFoundException: true);

            var AFIPAutenticacion = new Autenticacion
            (
                crt,
                pk,
                url,
                cuit
            );

            var wsaa = await AFIPAutenticacion.ObtenerToken(servicio);

            if (wsaa == null)
                return null;
            
            var afipAuth = new Domain.Entities.AfipAuth
            {
                IdEmpresa = idEmpresa,
                Sign = wsaa.Sign,
                Token = wsaa.Token,
                ExpirationTime = wsaa.Expiration,
                GenerationTime = wsaa.Generated,
                Servicio = servicio,
                CUIT = cuit
            };

            return afipAuth;

        }
    }
}
