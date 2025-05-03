using Dapper.DataTables.Models;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.SubdiarioIVACompras;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Application.Common.Extensions;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class SubdiarioIVAComprasBusiness: BaseBusiness, ISubdiarioIVAComprasBusiness
    {
        private readonly IComprobantesComprasRepository _comprobantesComprasRepository;

        public SubdiarioIVAComprasBusiness()
        {
        }

        public SubdiarioIVAComprasBusiness(IComprobantesComprasRepository comprobantesComprasRepository)
        {
            _comprobantesComprasRepository = comprobantesComprasRepository;
        }

        public async Task<List<Custom.DetalleAlicuota>> DetalleAlicuotas(DateTime? fechaDesde, DateTime? fechaHasta, int? idProveedor)
        {
            return await _comprobantesComprasRepository.GetDetalleAlicuotas(fechaDesde, fechaHasta, idProveedor, _permissionsBusiness.Value.User.IdEmpresa);
        }

        public async Task<List<Custom.ComprobanteCompraSubdiario>> GetAll(DataTablesRequest request)
        {
            var filters = request.ParseFilters();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            return await _comprobantesComprasRepository.GetAllSubdiarioCustomQuery(filters);
        }

        public async Task<string> GetCITICompras(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                if (!fechaDesde.HasValue)
                {
                    throw new BusinessException("Ingrese una Fecha Desde");
                }

                if (!fechaHasta.HasValue)
                {
                    throw new BusinessException("Ingrese una Fecha Hasta");
                }

                var lstCitiCompras = await _comprobantesComprasRepository.GetCITICompras(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                var citiCompras = _mapper.Map<List<CitiCompra>>(lstCitiCompras);

                var result = new StringBuilder();
                foreach (var citiVenta in citiCompras)
                {
                    result.AppendLine(citiVenta.ToStringAFIP());
                }

                return result.ToString();

            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public async Task<string> GetCITIComprasAli(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                if (!fechaDesde.HasValue)
                {
                    throw new BusinessException("Ingrese una Fecha Desde");
                }

                if (!fechaHasta.HasValue)
                {
                    throw new BusinessException("Ingrese una Fecha Hasta");
                }

                var lstCitiComprasAli = await _comprobantesComprasRepository.GetCITIComprasAli(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                var citiComprasAli = _mapper.Map<List<CitiCompraAlicuota>>(lstCitiComprasAli);

                var result = new StringBuilder();
                foreach (var citiCompraAli in citiComprasAli)
                {
                    result.AppendLine(citiCompraAli.ToStringAFIP());
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
    }
}
