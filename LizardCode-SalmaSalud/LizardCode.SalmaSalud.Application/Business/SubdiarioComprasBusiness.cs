using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Excel;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.SubdiarioCompras;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class SubdiarioComprasBusiness : BaseBusiness, ISubdiarioComprasBusiness
    {
        private readonly IComprobantesComprasRepository _comprobantesComprasRepository;

        public SubdiarioComprasBusiness(IComprobantesComprasRepository comprobantesComprasRepository)
        {
            _comprobantesComprasRepository = comprobantesComprasRepository;
        }

        public async Task<List<SubdiarioImputaciones>> DetalleImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int? idProveedor)
        {
            return await _comprobantesComprasRepository.GetSubdiarioImputaciones(fechaDesde, fechaHasta, idProveedor, _permissionsBusiness.Value.User.IdEmpresa);
        }

        public async Task<List<Custom.SubdiarioCompras>> GetAll(DataTablesRequest request)
        {
            var filters = request.ParseFilters();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            return await _comprobantesComprasRepository.GetSubdiarioCompras(filters);
        }

        public async Task<List<SubdiarioComprasDetalle>> GetDetalle(int idComprobanteCompra, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var filters = new Dictionary<string, object>();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            if (idComprobanteCompra == 0)
            {
                filters.Add("FechaDesde", fechaDesde.Value.ToString("dd/MM/yyyy"));
                filters.Add("FechaHasta", fechaHasta.Value.ToString("dd/MM/yyyy"));
            }
            else
            {
                filters.Add("IdComprobanteCompra", idComprobanteCompra);
            }

            return await _comprobantesComprasRepository.GetSubdiarioComprasDetalle(filters);
        }

        public async Task<byte[]> GetExcel(int idComprobanteCompra, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var detalle = await GetDetalle(idComprobanteCompra, fechaDesde, fechaHasta);

            if (detalle.Count > 0)
            {
                var detalleXls = _mapper.Map<List<SubdiarioComprasXLSViewModel>>(detalle);

                var eWrapper = new ExcelWrapper();
                eWrapper.AddListToBook(detalleXls);

                var excel = eWrapper.GetExcel();

                return excel;
            }
            else
                return null;
        }
    }
}
