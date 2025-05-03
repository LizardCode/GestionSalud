using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Excel;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.SubdiarioCobros;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class SubdiarioCobrosBusiness : BaseBusiness, ISubdiarioCobrosBusiness
    {
        private readonly IRecibosRepository _recibosRepository;

        public SubdiarioCobrosBusiness(IRecibosRepository recibosRepository)
        {
            _recibosRepository = recibosRepository;
        }

        public async Task<List<Custom.SubdiarioImputaciones>> DetalleImputaciones(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return await _recibosRepository.GetSubdiarioImputaciones(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
        }

        public async Task<List<Custom.SubdiarioCobros>> GetAll(DataTablesRequest request)
        {
            var filters = request.ParseFilters();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            return await _recibosRepository.GetSubdiarioCobros(filters);
        }

        public async Task<List<Custom.SubdiarioCobrosDetalle>> GetDetalle(int idRecibo, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var filters = new Dictionary<string, object>();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            if (idRecibo == 0)
            {
                filters.Add("FechaDesde", fechaDesde.Value.ToString("dd/MM/yyyy"));
                filters.Add("FechaHasta", fechaHasta.Value.ToString("dd/MM/yyyy"));
            }
            else
            {
                filters.Add("IdRecibo", idRecibo);
            }

            return await _recibosRepository.GetSubdiarioCobrosDetalle(filters);
        }

        public async Task<byte[]> GetExcel(int idRecibo, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var detalle = await GetDetalle(idRecibo, fechaDesde,  fechaHasta);

            if (detalle.Count > 0)
            {
                var detalleXls = _mapper.Map<List<SubdiarioCobrosXLSViewModel>>(detalle);

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
