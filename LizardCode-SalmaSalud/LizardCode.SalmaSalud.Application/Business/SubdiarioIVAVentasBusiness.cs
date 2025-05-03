using Dapper.DataTables.Models;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.SubdiarioIVAVentas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Application.Common.Extensions;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class SubdiarioIVAVentasBusiness: BaseBusiness, ISubdiarioIVAVentasBusiness
    {
        private readonly IComprobantesVentasRepository _comprobantesVentasRepository;

        public SubdiarioIVAVentasBusiness(IComprobantesVentasRepository comprobantesVentasRepository)
        {
            _comprobantesVentasRepository = comprobantesVentasRepository;
        }

        public async Task<List<Custom.DetalleAlicuota>> DetalleAlicuotas(DateTime? fechaDesde, DateTime? fechaHasta, int? idCliente)
        {
            return await _comprobantesVentasRepository.GetDetalleAlicuotas(fechaDesde, fechaHasta, idCliente, _permissionsBusiness.Value.User.IdEmpresa);
        }

        public async Task<List<Custom.ComprobanteVentaSubdiario>> GetAll(DataTablesRequest request)
        {
            var filters = request.ParseFilters();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            return await _comprobantesVentasRepository.GetAllSubdiarioCustomQuery(filters);
        }

        public async Task<string> GetCITIVentas(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                if(!fechaDesde.HasValue)
                {
                    throw new BusinessException("Ingrese una Fecha Desde");
                }

                if (!fechaHasta.HasValue)
                {
                    throw new BusinessException("Ingrese una Fecha Hasta");
                }

                var lstCitiVentas = await _comprobantesVentasRepository.GetCITIVentas(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                List<CitiVenta> citiVentas = _mapper.Map<List<CitiVenta>>(lstCitiVentas);

                var result = new StringBuilder();
                foreach (var citiVenta in citiVentas)
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

        public async Task<string> GetCITIVentasAli(DateTime? fechaDesde, DateTime? fechaHasta)
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

                var lstCitiVentasAli = await _comprobantesVentasRepository.GetCITIVentasAli(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                var citiVentasAli = _mapper.Map<List<CitiVentaAlicuota>>(lstCitiVentasAli);

                var result = new StringBuilder();
                foreach (var citiVentaAli in citiVentasAli)
                {
                    result.AppendLine(citiVentaAli.ToStringAFIP());
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
