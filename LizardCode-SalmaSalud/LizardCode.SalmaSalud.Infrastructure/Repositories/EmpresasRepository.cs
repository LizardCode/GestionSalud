using Dapper;
using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.AFIP;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using AFIP = LizardCode.Framework.Helpers.AFIP.Common.Clases;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class EmpresasRepository : BaseRepository, IEmpresasRepository, IDataTablesCustomQuery
    {
        public EmpresasRepository(IDbContext context) : base(context)
        {

        }

        public async Task<List<Empresa>> GetAllByIdUser(int idUsuario, IDbTransaction transaction = null)
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT DISTINCT
						e.*
                    FROM Empresas e
					INNER JOIN UsuariosEmpresas ue
						ON e.IdEmpresa = ue.IdEmpresa
                    WHERE ue.IdUsuario = {idUsuario}
                ");

            var result = await query.QueryAsync<Empresa>(transaction);

            return result.AsList();
        }

        public DataTablesCustomQuery GetAllCustomQuery()
        {
            var query = _context.Connection
                .QueryBuilder($@"
                    SELECT
						e.*,
						ti.Descripcion AS TipoIVA,
                        ec.Vencimiento AS VencimientoCertificado
                    FROM Empresas e
					INNER JOIN TipoIVA ti ON ti.IdTipoIVA = e.IdTipoIVA
                    LEFT JOIN (SELECT idEmpresa, MAX(Vencimiento) AS Vencimiento FROM EmpresasCertificados GROUP BY idEmpresa) ec ON ec.IdEmpresa = e.IdEmpresa
                    WHERE 
                        e.IdEstadoRegistro <> {(int)EstadoRegistro.Eliminado}
                ");

            return base.GetAllCustomQuery(query);
        }

        public async Task<Custom.Contribuyente> GetPadronByCUIT(AfipAuth afipAuth, string cuit, string cuitConsulta, bool useProd = false)
        {
            var urlKey = "PADRON-URL" + (useProd ? "" : "-DEV");
            var url = urlKey.FromAppSettings<string>(notFoundException: true);

            var datosContrib = new DatosContribuyente(afipAuth.Token, afipAuth.Sign, cuit, cuitConsulta);
            var persona = await datosContrib.GetPadron(url);

            if (!string.IsNullOrEmpty(persona.Errores))
                throw new Exception(persona.Errores);

            return new Custom.Contribuyente
            {
                Apellido = persona.Apellido,
                Nombre = persona.Nombre,
                RazonSocial = persona.RazonSocial,
                CodigoPostal = persona.CodigoPostal,
                CUIT = cuitConsulta,
                Direccion = persona.Direccion,
                Localidad = persona.Localidad,
                Provincia = persona.Provincia,
                Impuestos = persona.Impuestos
            };
        }

        public async Task<Custom.DatosComprobanteCompraAFIP> ValidComprobanteCompras(AfipAuth afipAuth, string cuit, int idComprobanteCompra, bool useProd)
        {
            var urlKey = "WSCDC-URL" + (useProd ? "" : "-DEV");
            var url = urlKey.FromAppSettings<string>(notFoundException: true);

            var builder = _context.Connection
                .QueryBuilder($@"
                    SELECT 
                        cc.*,
                        c.Codigo CbteTipo,
                        p.CUIT
                    FROM ComprobantesCompras cc 
                        INNER JOIN Comprobantes c ON cc.IdComprobante = c.IdComprobante
                        INNER JOIN Proveedores p ON cc.IdProveedor = p.IdProveedor
                        WHERE
                            cc.IdComprobanteCompra = {idComprobanteCompra}
                ");

            var comprobanteAFIP = await builder.QueryFirstOrDefaultAsync<Custom.ComprobanteCompraAFIP>();

            var comprobante = new ConsultaComprobantes(afipAuth.Token, afipAuth.Sign, cuit);
            comprobante.Conectar(url);

            var consulta = await comprobante.ConsutlaComprobantes(new AFIP.ComprobantesConsulta
            {
                CbteModo = "CAE",
                CbteFch = comprobanteAFIP.FechaReal,
                PtoVta = int.Parse(comprobanteAFIP.Sucursal),
                CbteNro = long.Parse(comprobanteAFIP.Numero),
                CbteTipo = comprobanteAFIP.CbteTipo,
                CodAutorizacion = comprobanteAFIP.CAE,
                CuitEmisor = long.Parse(comprobanteAFIP.CUIT.Replace("-", "")),
                DocNroReceptor = cuit,
                DocTipoReceptor = "80",
                ImpTotal = comprobanteAFIP.Total
            });

            return new Custom.DatosComprobanteCompraAFIP
            {
                Resultado = consulta.Resultado,
                Error = consulta.Error,
                Observacion = consulta.Observacion,
                XMLRequest = consulta.XMLRequest,
                XMLResponse = consulta.XMLResponse
            };
        }

        public async Task<Custom.DatosComprobanteVentaAFIP> GetConsultaComprobanteAFIP(AfipAuth afipAuth, string cuit, Custom.ComprobanteVentaAFIP comprobanteAFIP, bool useProd = false)
        {
            var urlKey = "WSFE-URL" + (useProd ? "" : "-DEV");
            var url = urlKey.FromAppSettings<string>(notFoundException: true);

            var comprobante = new ComprobantesElectronicos(afipAuth.Token, afipAuth.Sign, cuit);
            comprobante.Conectar(url);

            var consulta = await comprobante.GetCompConsultar(comprobanteAFIP.CbteTipo, int.Parse(comprobanteAFIP.Sucursal), int.Parse(comprobanteAFIP.Numero));

            return new Custom.DatosComprobanteVentaAFIP
            {
                Resultado = consulta.Resultado,
                Error = consulta.Error,
                Observacion = consulta.Observacion,
                CAE = consulta.NroCAE,
                VencimientoCAE = consulta.CAEFchVto,
                XMLRequest = consulta.XMLRequest,
                XMLResponse = consulta.XMLResponse,
            };

        }

        public async Task<Custom.DatosComprobanteVentaAFIP> ValidComprobanteVentas(AfipAuth afipAuth, string cuit, Custom.ComprobanteVentaAFIP comprobanteAFIP, bool useProd = false)
        {
            var urlKey = "WSFE-URL" + (useProd ? "" : "-DEV");
            var url = urlKey.FromAppSettings<string>(notFoundException: true);

            var comprobante = new ComprobantesElectronicos(afipAuth.Token, afipAuth.Sign, cuit);
            comprobante.Conectar(url);

            var consulta = await comprobante.CAESolicitarAsync(new AFIP.Comprobantes
            {
                CbteFch = comprobanteAFIP.Fecha,
                PtoVta = int.Parse(comprobanteAFIP.Sucursal),
                CbteDesde = long.Parse(comprobanteAFIP.Numero),
                CbteHasta = long.Parse(comprobanteAFIP.Numero),
                CbteTipo = comprobanteAFIP.CbteTipo,
                ComprobantesAsociados = comprobanteAFIP.ComprobanteVentaCbtAsociadosAFIP.Select(ca => new AFIP.ComprobantesAsociados { CbteFch = ca.CbteFch, PtoVta = ca.PtoVta, Nro = ca.Nro, Tipo = ca.Tipo, Cuit = ca.Cuit}).ToList(),
                Concepto = comprobanteAFIP.Concepto,
                DocTipo = 80,
                DocNro = long.Parse(comprobanteAFIP.CUIT.Replace("-", string.Empty)),
                FchServDesde = comprobanteAFIP.FchServDesde,
                FchServHasta = comprobanteAFIP.FchServHasta,
                FchVtoPago = comprobanteAFIP.FechaVto.HasValue ? comprobanteAFIP.FechaVto.Value : comprobanteAFIP.EsCredito ? null : comprobanteAFIP.Fecha,
                ImpIVA = comprobanteAFIP.ComprobanteVentaItemsAFIP.Sum(i => i.Importe),
                ImpNeto = comprobanteAFIP.Subtotal,
                ImpOpEx = 0,
                ImpTotal = comprobanteAFIP.Total,
                ImpTotConc = 0,
                ImpTrib = 0,
                MonCotiz = comprobanteAFIP.Cotizacion,
                MonId = comprobanteAFIP.Moneda,
                Opcionales = comprobanteAFIP.ComprobanteVentaOpcionalesAFIP.Select(op => new AFIP.Opcionales { Id = op.Id, Valor = op.Valor}).ToList(),
                PeriodoAsocFchServDesde = comprobanteAFIP.PeriodoAsocFchServDesde,
                PeriodoAsocFchServHasta = comprobanteAFIP.PeriodoAsocFchServHasta,
                TipoAlic = comprobanteAFIP.ComprobanteVentaItemsAFIP.Select(s => new AFIP.Alicuotas { BaseImponible = s.BaseImponible, Id = s.Id, Importe = s.Importe }).ToList(),
                TipoTrib = new List<AFIP.Tributos>()
            });

            return new Custom.DatosComprobanteVentaAFIP
            {
                Resultado = consulta.Resultado,
                Error = consulta.Error,
                Observacion = consulta.Observacion,
                CAE = consulta.NroCAE,
                VencimientoCAE = consulta.CAEFchVto,
                XMLRequest = consulta.XMLRequest,
                XMLResponse = consulta.XMLResponse,
            };
        }
    }
}
