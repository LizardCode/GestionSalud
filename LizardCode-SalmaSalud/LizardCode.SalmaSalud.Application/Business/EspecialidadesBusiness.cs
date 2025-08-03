using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Especialidades;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class EspecialidadesBusiness : BaseBusiness, IEspecialidadesBusiness
    {
        private readonly IEspecialidadesRepository _especialidadesRepository;

        public EspecialidadesBusiness(IEspecialidadesRepository especialidadesRepository)
        {
            _especialidadesRepository = especialidadesRepository;
        }

        public async Task New(EspecialidadViewModel model)
        {
            var especialidad = _mapper.Map<Especialidades>(model);

            Validate(especialidad);

            especialidad.Descripcion = especialidad.Descripcion.ToUpper().Trim();
            especialidad.TurnosIntervalo = 30;

            var tran = _uow.BeginTransaction();

            await _especialidadesRepository.Insert(especialidad, tran);

            tran.Commit();
        }

        public async Task<EspecialidadViewModel> Get(int idEspecialidad)
        {
            var especialidad = await _especialidadesRepository.GetById<Especialidades>(idEspecialidad);

            if (especialidad == null)
                return null;

            var model = _mapper.Map<EspecialidadViewModel>(especialidad);

            return model;
        }

        public async Task<DataTablesResponse<Especialidades>> GetAll(DataTablesRequest request)
        {
            var customQuery = _especialidadesRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Especialidades>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(EspecialidadViewModel model)
        {
            var especialidad = _mapper.Map<Especialidades>(model);

            Validate(especialidad);

            var dbEspecialidad = await _especialidadesRepository.GetById<Especialidades>(especialidad.IdEspecialidad);

            if (dbEspecialidad == null)
            {
                throw new ArgumentException("Especialidad inexistente");
            }
            dbEspecialidad.Descripcion = especialidad.Descripcion.ToUpper().Trim();
            //especialidad.TurnosIntervalo = 30;

            using var tran = _uow.BeginTransaction();
            await _especialidadesRepository.Update(dbEspecialidad, tran);

            tran.Commit();
        }

        public async Task Remove(int idEspecialidad)
        {
            throw new NotImplementedException();
        }

        private void Validate(Especialidades especialidad)
        {
            if (especialidad.Descripcion.IsNull())
            {
                throw new BusinessException(nameof(especialidad.Descripcion));
            }
        }
    }
}