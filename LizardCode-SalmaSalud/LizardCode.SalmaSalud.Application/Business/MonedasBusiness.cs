using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Monedas;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class MonedasBusiness: BaseBusiness, IMonedasBusiness
    {
        private readonly IMonedasRepository _monedasRepository;

        public MonedasBusiness(IMonedasRepository monedasRepository)
        {
            _monedasRepository = monedasRepository;
        }


        public async Task New(MonedasViewModel model)
        {
            var moneda = _mapper.Map<Moneda>(model);

            Validate(moneda);

            moneda.Descripcion = moneda.Descripcion.ToUpper().Trim();
            moneda.Codigo = moneda.Codigo.ToUpper().Trim();
            moneda.Simbolo = moneda.Simbolo.ToUpper().Trim();
            moneda.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

            var tran = _uow.BeginTransaction();

            await _monedasRepository.Insert(moneda, tran);

            tran.Commit();
        }

        public async Task<MonedasViewModel> Get(int idMoneda)
        {
            var moneda = await _monedasRepository.GetById<Moneda>(idMoneda);

            if (moneda == null)
                return null;

            var model = _mapper.Map<MonedasViewModel>(moneda);

            return model;
        }

        public async Task<DataTablesResponse<Moneda>> GetAll(DataTablesRequest request) =>
            await _dataTablesService.Resolve<Moneda>(request);
        
        public async Task Update(MonedasViewModel model)
        {
            var moneda = _mapper.Map<Moneda>(model);

            Validate(moneda);

            var dbMoneda = await _monedasRepository.GetById<Moneda>(moneda.IdMoneda);

            if (dbMoneda == null)
            {
                throw new ArgumentException("Moneda inexistente");
            }

            dbMoneda.Descripcion = moneda.Descripcion.ToUpper().Trim();
            dbMoneda.Codigo = moneda.Codigo.ToUpper().Trim();
            dbMoneda.Simbolo = moneda.Simbolo.ToUpper().Trim();
            dbMoneda.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            using var tran = _uow.BeginTransaction();
            await _monedasRepository.Update(dbMoneda, tran);

            tran.Commit();
        }

        public async Task Remove(int idMoneda)
        {
            var moneda = await _monedasRepository.GetById<Moneda>(idMoneda);

            if (moneda == null)
            {
                throw new ArgumentException("Moneda inexistente");
            }

            moneda.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _monedasRepository.Update(moneda);
        }

        private void Validate(Moneda moneda)
        {
            if (moneda.Descripcion.IsNull())
            {
                throw new BusinessException(nameof(moneda.Descripcion));
            }

            if (moneda.Codigo.IsNull())
            {
                throw new BusinessException(nameof(moneda.Codigo));
            }
        }
        public async Task<DataTablesResponse<Custom.Dashboard.TipoDeCambio>> GetTiposDeCambio(DataTablesRequest request)
        {
            var customQuery = _monedasRepository.GetTiposDeCambio();
            var builder = _dbContext.Connection.QueryBuilder();

            return await _dataTablesService.Resolve<Custom.Dashboard.TipoDeCambio>(request, customQuery.Sql, customQuery.Parameters, staticWhere: builder.Sql);
        }
    }
}
