using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Alicuotas;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class AlicuotasBusiness : BaseBusiness, IAlicuotasBusiness
    {
        private readonly IAlicuotasRepository _alicuotasRepository;

        public AlicuotasBusiness(IAlicuotasRepository alicuotasRepository)
        {
            _alicuotasRepository = alicuotasRepository;
        }


        public async Task New(AlicuotasViewModel model)
        {
            var alicuota = _mapper.Map<Alicuota>(model);

            Validate(alicuota);

            alicuota.Descripcion = alicuota.Descripcion.ToUpper().Trim();
            alicuota.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

            var tran = _uow.BeginTransaction();

            await _alicuotasRepository.Insert(alicuota, tran);

            tran.Commit();
        }

        public async Task<AlicuotasViewModel> Get(int idAlicuota)
        {
            var alicuota = await _alicuotasRepository.GetById<Alicuota>(idAlicuota);

            if (alicuota == null)
                return null;

            var model = _mapper.Map<AlicuotasViewModel>(alicuota);

            return model;
        }

        public async Task<DataTablesResponse<Custom.Alicuota>> GetAll(DataTablesRequest request)
        {
            var customQuery = _alicuotasRepository.GetAllCustomQuery();
            return await _dataTablesService.Resolve<Custom.Alicuota>(request, customQuery.Sql, customQuery.Parameters);
        }

        public async Task Update(AlicuotasViewModel model)
        {
            var alicuota = _mapper.Map<Alicuota>(model);

            Validate(alicuota);

            var dbAlicuota = await _alicuotasRepository.GetById<Alicuota>(alicuota.IdAlicuota);

            if (dbAlicuota == null)
            {
                throw new BusinessException("Alicuota inexistente");
            }

            dbAlicuota.Descripcion = alicuota.Descripcion.ToUpper().Trim();
            dbAlicuota.IdTipoAlicuota = alicuota.IdTipoAlicuota;
            dbAlicuota.Valor = alicuota.Valor;
            dbAlicuota.CodigoAFIP = alicuota.CodigoAFIP;
            dbAlicuota.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            using var tran = _uow.BeginTransaction();
            await _alicuotasRepository.Update(dbAlicuota, tran);

            tran.Commit();
        }

        public async Task Remove(int idAlicuota)
        {
            var alicuota = await _alicuotasRepository.GetById<Alicuota>(idAlicuota);

            if (alicuota == null)
            {
                throw new BusinessException("Alicuota inexistente");
            }

            alicuota.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _alicuotasRepository.Update(alicuota);
        }

        private void Validate(Alicuota alicuota)
        {
            if (alicuota.Descripcion.IsNull())
            {
                throw new BusinessException("Ingrese una Descripción para la Alicuota");
            }
        }
    }
}
