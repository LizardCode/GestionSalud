using Dapper.DataTables.Models;
using Microsoft.AspNetCore.Http;
using LizardCode.SalmaSalud.Application.Models.CargaManual;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ICargaManualBusiness
    {
        Task<CargaManualViewModel> Get(int idComprobanteCompra);
        Task<DataTablesResponse<Custom.ComprobanteCompra>> GetAll(DataTablesRequest request);
        Task New(CargaManualViewModel model);
        Task Remove(int idComprobanteCompra);
        Task Update(CargaManualViewModel model);
        Task<Custom.ComprobanteCompraManual> GetCustom(int idComprobanteCompra);
        Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id);
        Task<Custom.DatosComprobanteCompraAFIP> ValidateComprobante(int idComprobanteCpa);
        Task<List<string>> ProcesarCSV(IFormFile file, DateTime fechaInterfaz, int idEjercicioInterfaz, int IdCuentaContable);
        Task<List<string>> ProcesarCustomDawa(IFormFile file, DateTime fechaInterfaz, int idEjercicioInterfaz);
        Task<Custom.ComprobanteCompraManual> GetItemsPagosCustom(int idComprobanteCpa);
    }
}