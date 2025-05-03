using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Articulos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class ArticulosBusiness: BaseBusiness, IArticulosBusiness
    {
        private readonly ILogger<ArticulosBusiness> _logger;
        private readonly IArticulosRepository _articulosRepository;

        public ArticulosBusiness(IArticulosRepository articulosRepository, ILogger<ArticulosBusiness> logger)
        {
            _articulosRepository = articulosRepository;
            _logger = logger;
        }

        public async Task New(ArticulosViewModel model)
        {
            var articulo = _mapper.Map<Articulo>(model);

            Validate(articulo);

            var tran = _uow.BeginTransaction();

            try
            {
                articulo.Descripcion = articulo.Descripcion.ToUpper().Trim();
                articulo.CodigoBarras = articulo.CodigoBarras?.ToUpper().Trim() ?? String.Empty;
                articulo.Detalle = articulo.Detalle?.ToUpper().Trim() ?? String.Empty;
                articulo.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                articulo.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                await _articulosRepository.Insert(articulo, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
            }
        }

        public async Task<ArticulosViewModel> Get(int idArticulo)
        {
            var articulo = await _articulosRepository.GetById<Articulo>(idArticulo);

            if (articulo == null)
                return null;

            var model = _mapper.Map<ArticulosViewModel>(articulo);

            return model;
        }

        public async Task<DataTablesResponse<Articulo>> GetAll(DataTablesRequest request)
        {
            var customQuery = _articulosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Articulo>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }            

        public async Task Update(ArticulosViewModel model)
        {
            var articulo = _mapper.Map<Articulo>(model);

            Validate(articulo);

            var dbArticulo = await _articulosRepository.GetById<Articulo>(articulo.IdArticulo);

            if (dbArticulo == null)
            {
                throw new BusinessException("Articulo inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbArticulo.IdRubroArticulo = articulo.IdRubroArticulo;
                dbArticulo.Descripcion = articulo.Descripcion.ToUpper().Trim();
                dbArticulo.CodigoBarras = articulo.CodigoBarras?.ToUpper().Trim() ?? String.Empty;
                dbArticulo.Detalle = articulo.Detalle?.ToUpper().Trim() ?? String.Empty;
                dbArticulo.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                dbArticulo.Alicuota = articulo.Alicuota;
                dbArticulo.IdCuentaContableVentas = articulo.IdCuentaContableVentas;
                dbArticulo.IdCuentaContableCompras = articulo.IdCuentaContableCompras;
                dbArticulo.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _articulosRepository.Update(dbArticulo, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
            }
        }

        public async Task Remove(int idArticulo)
        {
            var articulo = await _articulosRepository.GetById<Articulo>(idArticulo);

            if (articulo == null)
            {
                throw new BusinessException("Articulo inexistente");
            }

            articulo.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _articulosRepository.Update(articulo);
        }


        private void Validate(Articulo articulo)
        {
            if (articulo.Descripcion.IsNull())
            {
                throw new BusinessException($"Ingrese una Descripción para el Articulo");
            }
        }

    }
}
