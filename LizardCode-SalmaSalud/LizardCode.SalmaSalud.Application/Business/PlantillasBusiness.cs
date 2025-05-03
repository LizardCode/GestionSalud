using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Plantillas;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;


namespace LizardCode.SalmaSalud.Application.Business
{
    public class PlantillasBusiness: BaseBusiness, IPlantillasBusiness
    {
        private readonly ILogger<PlantillasBusiness> _logger;
        private readonly IPlantillasRepository _plantillasRepository;
        private readonly IPlantillasDetalleRepository _plantillasDetalleRepository;

        public PlantillasBusiness(
            ILogger<PlantillasBusiness> logger,
            IPlantillasRepository plantillasRepository,
            IPlantillasDetalleRepository plantillasDetalleRepository)
        {
            _logger = logger;
            _plantillasRepository = plantillasRepository;
            _plantillasDetalleRepository = plantillasDetalleRepository;
        }

        public async Task<PlantillaViewModel> Get(int idPlantilla)
        {
            var plantilla = await _plantillasRepository.GetById<Plantilla>(idPlantilla);

            if (plantilla == null)
                return null;

            var model = _mapper.Map<PlantillaViewModel>(plantilla);

            return model;
        }

        public async Task<Custom.Plantilla> GetCustom(int idPlantilla)
            => await _plantillasRepository.GetById<Custom.Plantilla>(idPlantilla);

        public async Task<IList<Plantilla>> GetAll()
            => await _plantillasRepository.GetAll<Plantilla>();

        public async Task<DataTablesResponse<Custom.Plantilla>> GetAllDT(DataTablesRequest request)
        {
            var customQuery = _plantillasRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Plantilla>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task New(PlantillaViewModel model)
        {
            var plantilla = _mapper.Map<Plantilla>(model);

            await Validate(plantilla);

            var tran = _uow.BeginTransaction();

            try
            {
                plantilla.Descripcion = plantilla.Descripcion.ToUpper().Trim();
                plantilla.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                plantilla.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                if (model.FilePDF != null && model.FilePDF.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        model.FilePDF.CopyTo(ms);
                        plantilla.PDF = ms.ToArray();
                    }
                }


                if (model.FileJSON != null && model.FileJSON.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        model.FileJSON.CopyTo(ms);
                        plantilla.JSON = ms.ToArray();
                    }
                }

                plantilla.Top = 0;
                plantilla.Left = 0;
                plantilla.Width = 0;
                plantilla.Height = 0;

                var id = await _plantillasRepository.Insert(plantilla, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();

            }
        }

        public async Task Remove(int idPlantilla)
        {
            var plantilla = await _plantillasRepository.GetById<Plantilla>(idPlantilla);

            if (plantilla == null)
            {
                throw new BusinessException("Plantilla inexistente");
            }

            plantilla.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _plantillasRepository.Update(plantilla);
        }

        public async Task Update(PlantillaViewModel model)
        {
            var plantilla = _mapper.Map<Plantilla>(model);

            await Validate(plantilla);

            var dbPlantilla = await _plantillasRepository.GetById<Plantilla>(plantilla.IdPlantilla);

            if (dbPlantilla == null)
            {
                throw new BusinessException("Plantilla inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbPlantilla.IdTipoPlantilla = plantilla.IdTipoPlantilla;
                dbPlantilla.Descripcion = plantilla.Descripcion.ToUpper().Trim();
                dbPlantilla.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                if (model.FilePDF != null && model.FilePDF.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        model.FilePDF.CopyTo(ms);
                        dbPlantilla.PDF = ms.ToArray();
                    }
                }

                if (model.FileJSON != null && model.FileJSON.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        model.FileJSON.CopyTo(ms);
                        dbPlantilla.JSON = ms.ToArray();
                    }
                }

                dbPlantilla.Top = 0;
                dbPlantilla.Left = 0;
                dbPlantilla.Width = 0;
                dbPlantilla.Height = 0;

                await _plantillasRepository.Update(dbPlantilla, tran);

                await _plantillasDetalleRepository.DeleteByIdPlantilla(dbPlantilla.IdPlantilla, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();

            }

        }
        private async Task Validate(Plantilla plantilla)
        {
            if (plantilla.Descripcion.IsNull())
            {
                throw new BusinessException("Ingrese una Descripción para la Plantilla.");
            }

            var plantillasDb = await _plantillasRepository.GetPlantillasByTipo(_permissionsBusiness.Value.User.IdEmpresa, plantilla.IdTipoPlantilla);
            if (plantillasDb != null && plantillasDb.Count > 0 && plantillasDb.Any(p => p.IdPlantilla != plantilla.IdPlantilla))
            {
                throw new BusinessException("Existe una Plantilla de este tipo dada de Alta.");
            }
        }

        public async Task<List<PlantillaEtiqueta>> GetPlantillaEtiquetas(int idTipoPlantilla)
            => await _plantillasRepository.GetPlantillaEtiquetasByTipo(idTipoPlantilla);

        public async Task<string> ValidarTipoPlantilla(int idTipoPlantilla, int? idPlantilla)
        {
            var plantillasDb = await _plantillasRepository.GetPlantillasByTipo(_permissionsBusiness.Value.User.IdEmpresa, idTipoPlantilla);
            if (plantillasDb != null && plantillasDb.Count > 0)
            {
                if (idPlantilla.HasValue)
                {
                    if (plantillasDb.Any(p => p.IdPlantilla != idPlantilla.Value)) 
                    {
                        return new string("Existe una Plantilla de este tipo dada de Alta.");
                    }
                }
                else
                {
                    return new string("Existe una Plantilla de este tipo dada de Alta.");
                }
            }

            return null;
        }
    }
}
