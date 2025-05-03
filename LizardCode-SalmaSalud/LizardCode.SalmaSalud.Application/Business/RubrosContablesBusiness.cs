using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.RubrosContables;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class RubrosContablesBusiness: BaseBusiness, IRubrosContablesBusiness
    {
        private readonly IRubrosContablesRepository _rubrosContablesRepository;


        public RubrosContablesBusiness(IRubrosContablesRepository rubrosContablesRepository)
        {            
            _rubrosContablesRepository = rubrosContablesRepository;
        }


        public async Task New(RubrosContablesViewModel model)
        {
            var rubros = _mapper.Map<RubroContable>(model);

            Validate(rubros);

            rubros.CodigoRubro = rubros.CodigoRubro.PadRight(6, '0').ToUpper().Trim();
            rubros.Descripcion = rubros.Descripcion.ToUpper().Trim();
            rubros.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
            rubros.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

            var tran = _uow.BeginTransaction();

            await _rubrosContablesRepository.Insert(rubros, tran);

            tran.Commit();
        }

        public async Task<RubrosContablesViewModel> Get(int idRubro)
        {
            var rubro = await _rubrosContablesRepository.GetById<RubroContable>(idRubro);

            if (rubro == null)
                return null;

            RubroContable rubroPadre = default;
            if (rubro.IdRubroPadre.HasValue)
                rubroPadre = await _rubrosContablesRepository.GetById<RubroContable>(rubro.IdRubroPadre.Value);

            var model = _mapper.Map<RubrosContablesViewModel>(rubro);
            model.RubroPadre = rubroPadre?.Descripcion;

            return model;
        }

        public async Task<DataTablesResponse<Custom.RubroContable>> GetAll(DataTablesRequest request)
        {
            var customQuery = _rubrosContablesRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.RubroContable>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(RubrosContablesViewModel model)
        {
            var rubro = _mapper.Map<RubroContable>(model);

            Validate(rubro);

            var dbRubro = await _rubrosContablesRepository.GetById<RubroContable>(rubro.IdRubroContable);

            if (dbRubro == null)
            {
                throw new BusinessException("Rubro Contable inexistente");
            }

            if(dbRubro.IdRubroPadre == default)
            {
                throw new BusinessException($"El Rubro Contable {dbRubro.Descripcion} no se puede Modificar.");
            }

            dbRubro.CodigoRubro = rubro.CodigoRubro.PadRight(6, '0').ToUpper().Trim();
            dbRubro.Descripcion = rubro.Descripcion.ToUpper().Trim();
            dbRubro.IdRubroPadre = rubro.IdRubroPadre;
            dbRubro.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
            dbRubro.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            using var tran = _uow.BeginTransaction();
            await _rubrosContablesRepository.Update(dbRubro, tran);

            tran.Commit();
        }

        public async Task Remove(int idRubro)
        {
            var rubro = await _rubrosContablesRepository.GetById<RubroContable>(idRubro);

            if (rubro == null)
            {
                throw new BusinessException("Rubro Contable inexistente");
            }

            if (rubro.IdRubroPadre == default)
            {
                throw new BusinessException($"El Rubro Contable {rubro.Descripcion} no se puede Eliminar.");
            }

            rubro.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _rubrosContablesRepository.Update(rubro);
        }

        private void Validate(RubroContable rubro)
        {
            if (rubro.Descripcion.IsNull())
            {
                throw new BusinessException("Ingrese la Descripción del Rubro Contable");
            }

            if (rubro.CodigoRubro.IsNull())
            {
                throw new BusinessException("Ingrese el Código del Rubro Contable");
            }
        }

        public Task<List<Custom.Select2Custom>> GetRubrosContables(string q)
            => _rubrosContablesRepository.GetRubrosContablesByIdEmpresaAndTerm(_permissionsBusiness.Value.User.IdEmpresa, q);
    }
}
