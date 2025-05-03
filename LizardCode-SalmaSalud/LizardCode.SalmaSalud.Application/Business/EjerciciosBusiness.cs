using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Ejercicios;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class EjerciciosBusiness: BaseBusiness, IEjerciciosBusiness
    {
        private readonly ILogger<EjerciciosBusiness> _logger;
        private readonly IEjerciciosRepository _ejerciciosRepository;
        private readonly ICierreMesRepository _cierreMesRepository;

        public EjerciciosBusiness(
            ILogger<EjerciciosBusiness> logger, 
            ICierreMesRepository cierreMesRepository,
            IEjerciciosRepository ejerciciosRepository)
        {
            _logger = logger;
            _ejerciciosRepository = ejerciciosRepository;
            _cierreMesRepository = cierreMesRepository;
        }


        public async Task New(EjerciciosViewModel model)
        {
            var ejercicio = _mapper.Map<Ejercicio>(model);
            var codEjercicio = await _ejerciciosRepository.GetLastCodigoByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa);

            Validate(model);

            var tran = _uow.BeginTransaction();

            try
            {

                var mesHasta = int.Parse(model.MesAnnoInicio.Split('/')[0]);
                var annoHasta = int.Parse(model.MesAnnoInicio.Split('/')[1]);

                ejercicio.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                ejercicio.Codigo = codEjercicio == default ? "0001" : (int.Parse(codEjercicio) + 1).ToString().PadLeft(4, '0');
                ejercicio.FechaInicio = DateTime.ParseExact(string.Concat("01", "/", model.MesAnnoInicio), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                ejercicio.FechaFin = DateTime.ParseExact(string.Concat(DateTime.DaysInMonth(annoHasta, mesHasta), "/", model.MesAnnoFin), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                ejercicio.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                ejercicio.Cerrado = Commons.No.Description();

                var id = await _ejerciciosRepository.Insert(ejercicio, tran);

                var annoMesInicio = int.Parse(ejercicio.FechaInicio.ToString("yyyyMM"));
                var annoMesFin = int.Parse(ejercicio.FechaFin.ToString("yyyyMM"));

                var modulos = new List<string>() { Modulos.Clientes.Description(), Modulos.Proveedores.Description(), Modulos.CajaBancos.Description() };

                var mesesEjercicioDates = Enumerable.Range(0, 12)
                    .Select(i => ejercicio.FechaInicio.AddMonths(i))
                    .OrderBy(e => e)
                    .ToList();

                foreach (var mesEjercicio in mesesEjercicioDates)
                {
                    foreach (var modulo in modulos)
                    {
                        await _cierreMesRepository.Insert(new CierreMes
                        {
                            IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                            IdEjercicio = (int)id,
                            Anno = int.Parse(mesEjercicio.ToString("yyyy")),
                            Mes = int.Parse(mesEjercicio.ToString("MM")),
                            Modulo = modulo,
                            Cierre = Commons.No.Description()
                        }, tran);
                    }
                }

                tran.Commit();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task<EjerciciosViewModel> Get(int idEjercicio)
        {
            var ejercicio = await _ejerciciosRepository.GetById<Ejercicio>(idEjercicio);

            if (ejercicio == null)
                return null;

            var model = _mapper.Map<EjerciciosViewModel>(ejercicio);

            return model;
        }

        public async Task<DataTablesResponse<Ejercicio>> GetAll(DataTablesRequest request)
        {
            var customQuery = _ejerciciosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Ejercicio>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        private void Validate(EjerciciosViewModel model)
        {
            var rgValFormat = new Regex("^[0-9]{2}/[0-9]{4}$");
            if (!rgValFormat.IsMatch(model.MesAnnoInicio))
            {
                throw new BusinessException("Formato incorrecto de Fecha Inicio. mm/aaaa");
            }

            if (!rgValFormat.IsMatch(model.MesAnnoFin))
            {
                throw new BusinessException("Formato incorrecto de Fecha Fin. mm/aaaa");
            }
        }

        public async Task<string> ValidarFecha(string mesAnno, bool esFechaFin)
        {
            var rgValFormat = new Regex("^[0-9]{2}/[0-9]{4}$");
            if (!rgValFormat.IsMatch(mesAnno))
                return new string("Formato Incorrecto");

            if(!int.TryParse(mesAnno.Split('/')[0], out int mes))
            {
                return new string("Mes Incorrecto");
            }
            if (mes < 1 && mes > 12)
            {
                return new string("Mes Incorrecto");
            }

            if (!int.TryParse(mesAnno.Split('/')[1], out int anno))
            {
                return new string("Año Incorrecto");
            }

            var fecha = DateTime.ParseExact(string.Concat("01", "/", mesAnno), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            if (esFechaFin)
            {
                fecha = DateTime.ParseExact(string.Concat(DateTime.DaysInMonth(anno, mes), "/", mesAnno), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            var result = await _ejerciciosRepository.ValidateFechaEnOtroEjercicio(fecha, _permissionsBusiness.Value.User.IdEmpresa);
            if (result)
                return new string("Existe un Ejercicio con la Fecha Ingresada. Verifique");
            return null;
        }
    }
}
