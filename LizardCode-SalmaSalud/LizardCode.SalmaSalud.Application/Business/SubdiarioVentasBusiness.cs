using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Excel;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.SubdiarioVentas;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class SubdiarioVentasBusiness : BaseBusiness, ISubdiarioVentasBusiness
    {
        private readonly IComprobantesVentasRepository _comprobantesVentasRepository;

        public SubdiarioVentasBusiness(IComprobantesVentasRepository comprobantesVentasRepository)
        {
            _comprobantesVentasRepository = comprobantesVentasRepository;
        }

        public async Task<List<SubdiarioImputaciones>> DetalleImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int? idCliente)
        {
            return await _comprobantesVentasRepository.GetSubdiarioImputaciones(fechaDesde, fechaHasta, idCliente, _permissionsBusiness.Value.User.IdEmpresa);
        }

        public async Task<List<Custom.SubdiarioVentas>> GetAll(DataTablesRequest request)
        {
            var filters = request.ParseFilters();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            return await _comprobantesVentasRepository.GetSubdiarioVentas(filters);
        }

        public async Task<List<SubdiarioVentasDetalle>> GetDetalle(int idComprobanteVenta, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var filters = new Dictionary<string, object>();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            if (idComprobanteVenta == 0)
            {
                filters.Add("FechaDesde", fechaDesde.Value.ToString("dd/MM/yyyy"));
                filters.Add("FechaHasta", fechaHasta.Value.ToString("dd/MM/yyyy"));
            }
            else
            {
                filters.Add("IdComprobanteVenta", idComprobanteVenta);
            }

            return await _comprobantesVentasRepository.GetSubdiarioVentasDetalle(filters);
        }

        public async Task<byte[]> GetExcel(int idComprobanteVenta, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var detalle = await GetDetalle(idComprobanteVenta, fechaDesde,  fechaHasta);

            if (detalle.Count > 0)
            {
                var detalleXls = _mapper.Map<List<SubdiarioVentasXLSViewModel>>(detalle);

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
