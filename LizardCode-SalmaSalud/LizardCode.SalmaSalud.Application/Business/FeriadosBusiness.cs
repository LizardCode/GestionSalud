using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Feriados;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class FeriadosBusiness : BaseBusiness, IFeriadosBusiness
    {
        private readonly IFeriadosRepository _feriadosRepository;

        public FeriadosBusiness(IFeriadosRepository feriadosRepository)
        {
            _feriadosRepository = feriadosRepository;
        }

        public async Task New(FeriadoViewModel model)
        {
            var feriado = _mapper.Map<Feriado>(model);

            Validate(feriado);

            feriado.Nombre = feriado.Nombre.ToUpper().Trim();
            //feriado.Descripcion = feriado.Descripcion.ToUpper().Trim();
            feriado.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

            feriado.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;

            var tran = _uow.BeginTransaction();

            await _feriadosRepository.Insert(feriado, tran);

            tran.Commit();
        }

        public async Task<FeriadoViewModel> Get(int idFeriado)
        {
            var feriado = await _feriadosRepository.GetById<Feriado>(idFeriado);

            if (feriado == null)
                return null;

            var model = _mapper.Map<FeriadoViewModel>(feriado);

            return model;
        }

        //public async Task<DataTablesResponse<Custom.Feriado>> GetAll(DataTablesRequest request) =>
        //    await _dataTablesService.Resolve<Custom.Feriado>(request);

        public async Task<DataTablesResponse<Custom.Feriado>> GetAll(DataTablesRequest request)
        {
            var customQuery = _feriadosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            //if (filters.ContainsKey("FiltroCUIT"))
            //    builder.Append($"AND CUIT = {filters["FiltroCUIT"]}");

            //if (filters.ContainsKey("FiltroNombre"))
            //    builder.Append($"AND Nombre LIKE {string.Concat("%", filters["FiltroNombre"], "%")}");

            builder.Append($"AND idEmpresa = { _permissionsBusiness.Value.User.IdEmpresa } ");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Feriado>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(FeriadoViewModel model)
        {
            var feriado = _mapper.Map<Feriado>(model);

            Validate(feriado);

            var dbFeriado = await _feriadosRepository.GetById<Feriado>(feriado.IdFeriado);

            if (dbFeriado == null)
            {
                throw new ArgumentException("Feriado inexistente");
            }

            dbFeriado.Nombre = feriado.Nombre.ToUpper().Trim();
            //dbFeriado.Descripcion = feriado.Descripcion.ToUpper().Trim();
            dbFeriado.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            using var tran = _uow.BeginTransaction();
            await _feriadosRepository.Update(dbFeriado, tran);

            tran.Commit();
        }

        public async Task Remove(int idFeriado)
        {
            var feriado = await _feriadosRepository.GetById<Feriado>(idFeriado);

            if (feriado == null)
            {
                throw new ArgumentException("Feriado inexistente");
            }

            feriado.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _feriadosRepository.Update(feriado);
        }

        private void Validate(Feriado feriado)
        {
            if (feriado.Nombre.IsNull())
            {
                throw new BusinessException(nameof(feriado.Nombre));
            }
        }
    }
}