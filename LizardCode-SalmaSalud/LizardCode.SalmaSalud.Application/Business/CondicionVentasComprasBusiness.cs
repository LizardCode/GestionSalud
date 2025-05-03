using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.CondicionVentasCompras;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class CondicionVentasComprasBusiness: BaseBusiness, ICondicionVentasComprasBusiness
    {
        private readonly ICondicionVentasComprasRepository _condicionVentasComprasRepository;

        public CondicionVentasComprasBusiness(ICondicionVentasComprasRepository condicionVentasComprasRepository)
        {
            _condicionVentasComprasRepository = condicionVentasComprasRepository;
        }

        public async Task New(CondicionVentasComprasViewModel model)
        {
            var condicion = _mapper.Map<CondicionVentaCompra>(model);

            Validate(condicion);

            condicion.Descripcion = condicion.Descripcion.ToUpper().Trim();
            condicion.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

            var tran = _uow.BeginTransaction();

            await _condicionVentasComprasRepository.Insert(condicion, tran);

            tran.Commit();
        }

        public async Task<CondicionVentasComprasViewModel> Get(int idCondicion)
        {
            var condicion = await _condicionVentasComprasRepository.GetById<CondicionVentaCompra>(idCondicion);

            if (condicion == null)
                return null;

            var model = _mapper.Map<CondicionVentasComprasViewModel>(condicion);

            return model;
        }

        public async Task<DataTablesResponse<Custom.CondicionVentaCompra>> GetAll(DataTablesRequest request)
        {
            var customQuery = _condicionVentasComprasRepository.GetAllCustomQuery();
            return await _dataTablesService.Resolve<Custom.CondicionVentaCompra>(request, customQuery.Sql, customQuery.Parameters);
        }

        public async Task Update(CondicionVentasComprasViewModel model)
        {
            var condicion = _mapper.Map<CondicionVentaCompra>(model);

            Validate(condicion);

            var dbCondicion = await _condicionVentasComprasRepository.GetById<CondicionVentaCompra>(condicion.IdCondicion);

            if (dbCondicion == null)
            {
                throw new BusinessException("Condición de Venta/Compra inexistente");
            }

            dbCondicion.Descripcion = condicion.Descripcion.ToUpper().Trim();
            dbCondicion.IdTipoCondicion = condicion.IdTipoCondicion;
            dbCondicion.Dias = condicion.Dias;
            dbCondicion.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            using var tran = _uow.BeginTransaction();
            await _condicionVentasComprasRepository.Update(dbCondicion, tran);

            tran.Commit();
        }

        public async Task Remove(int idCondicion)
        {
            var condicion = await _condicionVentasComprasRepository.GetById<CondicionVentaCompra>(idCondicion);

            if (condicion == null)
            {
                throw new BusinessException("Condición de Venta/Compra inexistente");
            }

            condicion.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _condicionVentasComprasRepository.Update(condicion);
        }

        private void Validate(CondicionVentaCompra condicion)
        {
            if(condicion.Descripcion.IsNull())
            {
                throw new BusinessException("Ingrese una Desrcipción");
            }

        }
    }
}
