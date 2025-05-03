using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesComprasTotalesRepository
    {
        Task<IList<ComprobanteCompraTotales>> GetAllByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null);

        Task<bool> Insert(ComprobanteCompraTotales entity, IDbTransaction transaction = null);

        Task<bool> Update(ComprobanteCompraTotales entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdComprobanteCompra(int idComprobanteCompra, IDbTransaction transaction = null);

        Task<bool> DeleteByIdComprobanteCompraAndAlicuota(int idComprobanteCompra, int item, IDbTransaction transaction = null);

        Task<ComprobanteCompraTotales> GetByIdComprobanteCompraAndAlicuota(int idComprobanteCompra, int item, IDbTransaction transaction = null);

        Task<List<Custom.ComprobanteCompraTotales>> GetTotalesByFechaPriUlt(DateTime fecha, int idEmpresa, int idProveedor, IDbTransaction transaction = null);

        Task<List<Custom.ComprobanteCompraTotales>> GetTotalesByIdOrdenPago(int idEmpresa, int idOrdenPago, IDbTransaction transaction = null);

        Task<List<Custom.ComprobanteCompraTotales>> GetTotalesByFechaMonotributo(DateTime fecha, int canMeses, int idEmpresa, int idProveedor, IDbTransaction transaction = null);

        Task<bool> RemoveAllByIdSdoCtaCtePrv(int idSdoCtaCtePrv, IDbTransaction transaction = null);
    }
}