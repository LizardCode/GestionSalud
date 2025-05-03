using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Consultorios;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    internal class ConsultoriosBusiness : BaseBusiness, IConsultoriosBusiness
    {
        private readonly IConsultoriosRepository _ConsultoriosRepository;

        public ConsultoriosBusiness(IConsultoriosRepository ConsultoriosRepository)
        {
            _ConsultoriosRepository = ConsultoriosRepository;
        }

        public async Task New(ConsultorioViewModel model)
        {
            var consultorio = _mapper.Map<Consultorio>(model);

            Validate(consultorio);

            consultorio.Descripcion = consultorio.Descripcion.ToUpper().Trim();
            consultorio.Edificio = consultorio.Edificio.ToUpper().Trim();
            consultorio.Piso = consultorio.Piso.ToUpper().Trim();
            consultorio.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

            consultorio.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;

            var tran = _uow.BeginTransaction();

            await _ConsultoriosRepository.Insert(consultorio, tran);

            tran.Commit();
        }

        public async Task<ConsultorioViewModel> Get(int idConsultorio)
        {
            var consultorio = await _ConsultoriosRepository.GetById<Consultorio>(idConsultorio);

            if (consultorio == null)
                return null;

            var model = _mapper.Map<ConsultorioViewModel>(consultorio);

            return model;
        }

        public async Task<DataTablesResponse<Consultorio>> GetAll(DataTablesRequest request)
        {
            var customQuery = _ConsultoriosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            builder.Append($"AND idEmpresa = {_permissionsBusiness.Value.User.IdEmpresa} ");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Consultorio>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(ConsultorioViewModel model)
        {
            var consultorio = _mapper.Map<Consultorio>(model);

            Validate(consultorio);

            var dbConsultorio = await _ConsultoriosRepository.GetById<Consultorio>(consultorio.IdConsultorio);

            if (dbConsultorio == null)
            {
                throw new ArgumentException("Consultorio inexistente");
            }

            dbConsultorio.Descripcion = consultorio.Descripcion.ToUpper().Trim();
            dbConsultorio.Edificio = consultorio.Edificio.ToUpper().Trim();
            dbConsultorio.Piso = consultorio.Piso.ToUpper().Trim();
            dbConsultorio.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            using var tran = _uow.BeginTransaction();
            await _ConsultoriosRepository.Update(dbConsultorio, tran);

            tran.Commit();
        }

        public async Task Remove(int idConsultorio)
        {
            var consultorio = await _ConsultoriosRepository.GetById<Consultorio>(idConsultorio);

            if (consultorio == null)
            {
                throw new ArgumentException("Consultorio inexistente");
            }

            consultorio.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _ConsultoriosRepository.Update(consultorio);
        }

        private void Validate(Consultorio Consultorio)
        {
            if (Consultorio.Descripcion.IsNull())
            {
                throw new BusinessException(nameof(Consultorio.Descripcion));
            }
        }
    }
}