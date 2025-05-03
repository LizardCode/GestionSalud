using Dapper.DataTables.Models;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using umeral1.Gestion.Application.Models.ListadoRetenciones;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.SalmaSalud.Application.Common.Extensions;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class ListadoRetencionesBusiness: BaseBusiness, IListadoRetencionesBusiness
    {
        private readonly IRetencionesPercepcionesRepository _retencionesPercepcionesRepository;

        public ListadoRetencionesBusiness(IRetencionesPercepcionesRepository retencionesPercepcionesRepository)
        {
            _retencionesPercepcionesRepository = retencionesPercepcionesRepository;
        }

        public async Task<List<Custom.RetencionPercepcion>> GetAll(DataTablesRequest request)
        {
            var filters = request.ParseFilters();

            if (filters.ContainsKey("IdTipoRetecion"))
            {
                filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

                switch (int.Parse(filters["IdTipoRetecion"].ToString()))
                {
                    case (int)ListaRetencionesPercepciones.GananciasProveedores:
                        return await _retencionesPercepcionesRepository.GetRetencionGananciasProveedores(filters);
                    case (int)ListaRetencionesPercepciones.IVAProveedores:
                        return await _retencionesPercepcionesRepository.GetRetencionIVAProveedores(filters);
                    case (int)ListaRetencionesPercepciones.IngresosBrutosProveedores:
                        return await _retencionesPercepcionesRepository.GetRetencionIngresosBrutosProveedores(filters);
                    case (int)ListaRetencionesPercepciones.SUSSProveedores:
                        return await _retencionesPercepcionesRepository.GetRetencionSUSSProveedores(filters);
                    case (int)ListaRetencionesPercepciones.PercepcionIngresosBrutosProveedores:
                        return await _retencionesPercepcionesRepository.GetPercepcionIngresosBrutosProveedores(filters);
                    case (int)ListaRetencionesPercepciones.GananciasClientes:
                        return await _retencionesPercepcionesRepository.GetRetencionGananciasClientes(filters);
                    case (int)ListaRetencionesPercepciones.IVAClientes:
                        return await _retencionesPercepcionesRepository.GetRetencionIVAClientes(filters);
                    case (int)ListaRetencionesPercepciones.IngresosBrutosClientes:
                        return await _retencionesPercepcionesRepository.GetRetencionIngresosBrutosClientes(filters);
                    case (int)ListaRetencionesPercepciones.SUSSClientes:
                        return await _retencionesPercepcionesRepository.GetRetencionSUSSClientes(filters);
                    case (int)ListaRetencionesPercepciones.PercepcionIngresosBrutosClientes:
                        return await _retencionesPercepcionesRepository.GetPercepcionIngresosBrutosClientes(filters);
                    default:
                        return new List<Custom.RetencionPercepcion>();
                }
            }
            else
                return new List<Custom.RetencionPercepcion>();
        }

        public async Task<string> GetSicore(int idTipoRetencion, DateTime? fechaDesde, DateTime? fechaHasta)
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

                var result = new StringBuilder();

                switch (idTipoRetencion)
                {
                    case (int)ListaRetencionesPercepciones.GananciasProveedores:
                        var lstSicoreGanProvee = await _retencionesPercepcionesRepository.GetGananciasProveedores(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                        var sicoreGanProvee = _mapper.Map<List<SicoreGananciasProveedores>>(lstSicoreGanProvee);
                        foreach (var retPer in sicoreGanProvee)
                            result.AppendLine(retPer.ToStringAFIP());
                        break;
                    case (int)ListaRetencionesPercepciones.IVAProveedores:
                        var lstSicoreIVAProvee = await _retencionesPercepcionesRepository.GetIVAProveedores(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                        var sicoreIVAProvee = _mapper.Map<List<SicoreIVAProveedores>>(lstSicoreIVAProvee);
                        foreach (var retPer in sicoreIVAProvee)
                            result.AppendLine(retPer.ToStringAFIP());
                        break;
                    case (int)ListaRetencionesPercepciones.IngresosBrutosProveedores:
                        var lstSicoreIBrProvee = await _retencionesPercepcionesRepository.GetIngresosBrutosProveedores(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                        var sicoreIBrProvee = _mapper.Map<List<SicoreIngresosBrutosProveedores>>(lstSicoreIBrProvee);
                        foreach (var retPer in sicoreIBrProvee)
                            result.AppendLine(retPer.ToStringAFIP());
                        break;
                    case (int)ListaRetencionesPercepciones.SUSSProveedores:
                        var lstSicoreSUSSProvee = await _retencionesPercepcionesRepository.GetSUSSProveedores(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                        var sicoreSUSSProvee = _mapper.Map<List<SicoreSUSSProveedores>>(lstSicoreSUSSProvee);
                        foreach (var retPer in sicoreSUSSProvee)
                            result.AppendLine(retPer.ToStringAFIP());
                        break;
                    case (int)ListaRetencionesPercepciones.PercepcionIngresosBrutosProveedores:
                        var lstSicorePerIbrProvee = await _retencionesPercepcionesRepository.GetIngresosBrutosProveedoresPercepcion(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                        var sicorePerIbrProvee = _mapper.Map<List<SicorePercepcionIngresosBrutosProveedores>>(lstSicorePerIbrProvee);
                        foreach (var retPer in sicorePerIbrProvee)
                            result.AppendLine(retPer.ToStringAFIP());
                        break;
                    case (int)ListaRetencionesPercepciones.GananciasClientes:
                        throw new BusinessException("El Proceso de Exportación de Retención de Ganancias de Clientes no esta Implementado. Comuniquese con el Administrador del Sistema.");
                        break;
                    case (int)ListaRetencionesPercepciones.IVAClientes:
                        //lstSicore = await _retencionesPercepcionesRepository.GetIVAClientes(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                        //sicore = _mapper.Map<List<SicoreIVAClientes>>(lstSicore);
                        throw new BusinessException("El Proceso de Exportación de Retención de IVA de Clientes no esta Implementado. Comuniquese con el Administrador del Sistema.");
                        break;
                    case (int)ListaRetencionesPercepciones.IngresosBrutosClientes:
                        //lstSicore = await _retencionesPercepcionesRepository.GetIngresosBrutosClientes(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                        //sicore = _mapper.Map<List<SicoreIngresosBrutosClientes>>(lstSicore);
                        throw new BusinessException("El Proceso de Exportación de Retención de Ingresos Brutos de Clientes no esta Implementado. Comuniquese con el Administrador del Sistema.");
                        break;
                    case (int)ListaRetencionesPercepciones.SUSSClientes:
                        //lstSicore = await _retencionesPercepcionesRepository.GetSUSSClientes(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                        //sicore = _mapper.Map<List<SicoreSUSSClientes>>(lstSicore);
                        throw new BusinessException("El Proceso de Exportación de Retención de S.U.S.S de Clientes no esta Implementado. Comuniquese con el Administrador del Sistema.");
                        break;
                    case (int)ListaRetencionesPercepciones.PercepcionIngresosBrutosClientes:
                        //lstSicore = await _retencionesPercepcionesRepository.GetIngresosBrutosClientesPercepcion(fechaDesde, fechaHasta, _permissionsBusiness.Value.User.IdEmpresa);
                        //sicore = _mapper.Map<List<SicorePercepcionIngresosBrutosClientes>>(lstSicore);
                        throw new BusinessException("El Proceso de Exportación de Percepción de Ingresos Brutos de Clientes no esta Implementado. Comuniquese con el Administrador del Sistema.");
                        break;
                    default:
                        throw new BusinessException("Error en el Tipo de Retención a Procesar.");
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
