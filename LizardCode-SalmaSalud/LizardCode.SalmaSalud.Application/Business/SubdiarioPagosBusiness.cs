using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Excel;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.SubdiarioPagos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class SubdiarioPagosBusiness : BaseBusiness, ISubdiarioPagosBusiness
    {
        private readonly IOrdenesPagoRepository _ordenesPagoRepository;

        public SubdiarioPagosBusiness(IOrdenesPagoRepository ordenesPagoRepository)
        {
			_ordenesPagoRepository = ordenesPagoRepository;
        }

        public async Task<List<Custom.SubdiarioImputaciones>> DetalleImputaciones(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return await _ordenesPagoRepository.GetSubdiarioImputaciones(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
        }

        public async Task<List<Custom.SubdiarioPagos>> GetAll(DataTablesRequest request)
        {
            var filters = request.ParseFilters();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            return await _ordenesPagoRepository.GetSubdiarioPagos(filters);
        }

        public async Task<List<Custom.SubdiarioPagosDetalle>> GetDetalle(int idOrdenPago, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var filters = new Dictionary<string, object>();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            if (idOrdenPago == 0)
            {
                filters.Add("FechaDesde", fechaDesde.Value.ToString("dd/MM/yyyy"));
                filters.Add("FechaHasta", fechaHasta.Value.ToString("dd/MM/yyyy"));
            }
            else
            {
                filters.Add("IdOrdenPago", idOrdenPago);
            }

            return await _ordenesPagoRepository.GetSubdiarioPagosDetalle(filters);
        }

        public async Task<byte[]> GetExcel(int idOrdenPago, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var detalle = await GetDetalle(idOrdenPago, fechaDesde, fechaHasta);

            if (detalle.Count > 0)
            {
                var detalleXls = _mapper.Map<List<SubdiarioPagosXLSViewModel>>(detalle);

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
