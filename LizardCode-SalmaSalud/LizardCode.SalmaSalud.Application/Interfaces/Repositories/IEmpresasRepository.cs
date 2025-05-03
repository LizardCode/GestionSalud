using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IEmpresasRepository
    {
        Task<IList<TEmpresa>> GetAll<TEmpresa>(IDbTransaction transaction = null);

        Task<TEmpresa> GetById<TEmpresa>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TEmpresa>(TEmpresa entity, IDbTransaction transaction = null);

        Task<bool> Update<TEmpresa>(TEmpresa entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<List<Domain.Entities.Empresa>> GetAllByIdUser(int idUsuario, IDbTransaction transaction = null);

        Task<Domain.EntitiesCustom.Contribuyente> GetPadronByCUIT(Domain.Entities.AfipAuth afipAuth, string cuit, string cuitConsulta, bool useProd);

        Task<Domain.EntitiesCustom.DatosComprobanteCompraAFIP> ValidComprobanteCompras(AfipAuth afipAuth, string cuit, int idComprobanteCompra, bool useProd);

        Task<Custom.DatosComprobanteVentaAFIP> ValidComprobanteVentas(AfipAuth afipAuth, string cuit, Custom.ComprobanteVentaAFIP comprobanteAFIP, bool useProd);

        Task<Custom.DatosComprobanteVentaAFIP> GetConsultaComprobanteAFIP(AfipAuth afipAuth, string cuit, Custom.ComprobanteVentaAFIP comprobanteAFIP, bool useProd = false);
    }
}