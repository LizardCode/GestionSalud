using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IRetencionesPercepcionesRepository
    {
        Task<List<Custom.RetencionPercepcion>> GetRetencionGananciasProveedores(Dictionary<string, object> filters);

        Task<List<Custom.RetencionPercepcion>> GetRetencionIngresosBrutosProveedores(Dictionary<string, object> filters);

        Task<List<Custom.RetencionPercepcion>> GetRetencionIVAProveedores(Dictionary<string, object> filters);

        Task<List<Custom.RetencionPercepcion>> GetRetencionSUSSProveedores(Dictionary<string, object> filters);

        Task<List<Custom.RetencionPercepcion>> GetPercepcionIngresosBrutosProveedores(Dictionary<string, object> filters);

        Task<List<Custom.RetencionPercepcion>> GetRetencionGananciasClientes(Dictionary<string, object> filters);

        Task<List<Custom.RetencionPercepcion>> GetRetencionIngresosBrutosClientes(Dictionary<string, object> filters);

        Task<List<Custom.RetencionPercepcion>> GetRetencionIVAClientes(Dictionary<string, object> filters);

        Task<List<Custom.RetencionPercepcion>> GetRetencionSUSSClientes(Dictionary<string, object> filters);

        Task<List<Custom.RetencionPercepcion>> GetPercepcionIngresosBrutosClientes(Dictionary<string, object> filters);

        Task<List<Custom.SicoreGananciasProveedores>> GetGananciasProveedores(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<List<Custom.SicoreIVAProveedores>> GetIVAProveedores(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<List<Custom.SicoreIngresosBrutosProveedores>> GetIngresosBrutosProveedores(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<List<Custom.SicoreSUSSProveedores>> GetSUSSProveedores(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<List<Custom.SicorePercepcionIngresosBrutosProveedores>> GetIngresosBrutosProveedoresPercepcion(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        //Task<dynamic> GetIVAClientes(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        //Task<List<Custom.SicorePercepcionIngresosBrutosProveedores>> GetIngresosBrutosClientes(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);
    }
}