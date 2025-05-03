using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.MonedasFechasCambio;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class MonedasFechasCambioBusiness: BaseBusiness, IMonedasFechasCambioBusiness
    {
        private readonly IMonedasRepository _monedasRepository;
        private readonly IMonedasFechasCambioRepository _monedasTiposCambioRepository;


        public MonedasFechasCambioBusiness(
            IMonedasFechasCambioRepository monedasTiposCambioRepository,
            IMonedasRepository monedasRepository)
        {
            _monedasRepository = monedasRepository;
            _monedasTiposCambioRepository = monedasTiposCambioRepository;
        }

        public async Task<MonedasFechasCambioViewModel> Get(int idMoneda)
        {
            var moneda = await _monedasRepository.GetById<Moneda>(idMoneda);

            if (moneda == null)
                return null;

            var model = _mapper.Map<MonedasFechasCambioViewModel>(moneda);

            return model;
        }

        public async Task<DataTablesResponse<Moneda>> GetAll(DataTablesRequest request)
        {
            var customQuery = _monedasRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdMoneda > {1}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Moneda>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(MonedasFechasCambioViewModel model)
        {
            var moneda = _mapper.Map<MonedaFechasCambio>(model);

            Validate(moneda);

            var dbMoneda = await _monedasTiposCambioRepository.GetByIdAndFecha(moneda.IdMoneda, moneda.Fecha, _permissionsBusiness.Value.User.IdEmpresa);

            using var tran = _uow.BeginTransaction();

            if (dbMoneda == null)
            {
                dbMoneda = new MonedaFechasCambio
                {
                    IdMoneda = moneda.IdMoneda,
                    Fecha = moneda.Fecha,
                    Cotizacion = moneda.Cotizacion,
                    IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa
                };

                await _monedasTiposCambioRepository.Insert(dbMoneda, tran);
            }
            else
            {
                dbMoneda.Cotizacion = moneda.Cotizacion;
                dbMoneda.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;

                
                await _monedasTiposCambioRepository.Update(dbMoneda, tran);

            }

            tran.Commit();

        }

        public async Task<double?> GetFechaCambio(int idMoneda, DateTime fecha) =>
            await _monedasTiposCambioRepository.GetFechaCambio(idMoneda, fecha, _permissionsBusiness.Value.User.IdEmpresa);
        

        private void Validate(MonedaFechasCambio moneda)
        {

        }

        public async Task<IList<MonedaFechasCambio>> GetCotizaciones(int idMoneda) =>
            await _monedasTiposCambioRepository.GetCotizaciones(idMoneda, _permissionsBusiness.Value.User.IdEmpresa);

    }
}
