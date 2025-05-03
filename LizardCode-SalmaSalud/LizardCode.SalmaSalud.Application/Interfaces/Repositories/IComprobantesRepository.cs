using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IComprobantesRepository
    {
        Task<IList<TComprobante>> GetAll<TComprobante>(IDbTransaction transaction = null);

        Task<TComprobante> GetById<TComprobante>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TComprobante>(TComprobante entity, IDbTransaction transaction = null);

        Task<bool> Update<TComprobante>(TComprobante entity, IDbTransaction transaction = null);
        
        DataTablesCustomQuery GetAllCustomQuery();

        Task<IList<Comprobante>> GetComprobantesByTipoIVA(int idTipoIVA);

        Task<IList<Comprobante>> GetComprobantesByCliente(int idCliente);

        Task<IList<Comprobante>> GetComprobantesByProveedor(int idProveedor);

        Task<IList<Comprobante>> GetComprobantesParaCredito();

        Task<IList<Comprobante>> GetComprobantes();

        Task<decimal> GetIVACompras(DateTime desde, DateTime hasta, int idEmpresa);

        Task<decimal> GetIVAVentas(DateTime desde, DateTime hasta, int idEmpresa);

        Task<int> GetCantidadFacturasVentas(DateTime desde, DateTime hasta, int idEmpresa);

        Task<int> GetCantidadFacturasCompras(DateTime desde, DateTime hasta, int idEmpresa);
        Task<int> GetCantidadFacturasComprasPagasProveedor(int idProveedor, int idEmpresa);
        Task<int> GetCantidadFacturasComprasProveedor(int idProveedor, int idEmpresa);
        DataTablesCustomQuery GetUltimasFacturasProveedor(int idProveedor, int idEmpresa);
    }
}