using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Cheques;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class ChequesBusiness: BaseBusiness, IChequesBusiness
    {
        private readonly ILogger<ChequesBusiness> _logger;
        private readonly IChequesRepository _chequesRepository;
        private readonly IBancosRepository _bancosRepository;
        private readonly IAsientosRepository _asientosRepository;
        private readonly IChequesDebitosAsientoRepository _chequesDebitosAsientoRepository;
        private readonly IAsientosDetalleRepository _asientosDetalleRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly ICierreMesRepository _cierreMesRepository;
        private readonly IOrdenesPagoRepository _ordenesPagoRepository;
        private readonly IDepositosBancoRepository _depositosBancoRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;

        public ChequesBusiness(
            IChequesRepository chequesRepository, 
            IBancosRepository bancosRepository, 
            ILogger<ChequesBusiness> logger, 
            IAsientosRepository asientosRepository, 
            IAsientosDetalleRepository asientosDetalleRepository,
            IChequesDebitosAsientoRepository chequesDebitosAsientoRepository,
            ICierreMesRepository cierreMesRepository,
            IEjerciciosRepository ejerciciosRepository,
            IOrdenesPagoRepository ordenesPagoRepository,
            IDepositosBancoRepository depositosBancoRepository,
            ICuentasContablesRepository cuentasContablesRepository
            )
        {
            _chequesRepository = chequesRepository;
            _bancosRepository = bancosRepository;
            _asientosRepository = asientosRepository;
            _asientosDetalleRepository = asientosDetalleRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
            _chequesDebitosAsientoRepository = chequesDebitosAsientoRepository;
            _cierreMesRepository = cierreMesRepository;
            _ordenesPagoRepository = ordenesPagoRepository;
            _depositosBancoRepository = depositosBancoRepository;
            _ejerciciosRepository = ejerciciosRepository;
            _logger = logger;
        }

        public async Task New(ChequesViewModel model)
        {
            Validate(model);
            
            var duplicadoRangoCheques = await _chequesRepository.GetVerificarDuplicados(_permissionsBusiness.Value.User.IdEmpresa, model.IdBanco, model.IdTipoCheque, model.NroDesde, model.NroHasta);
            if (duplicadoRangoCheques)
                throw new BusinessException("Error en el Rango de Cheques. La Numeración Ingresada ya Existe. Verifique. ");

            var tran = _uow.BeginTransaction();

            try
            {
                var banco = await _bancosRepository.GetById<Banco>(model.IdBanco, tran);

                for (int iLoop = int.Parse(model.NroDesde); iLoop <= int.Parse(model.NroHasta); iLoop++)
                {
                    await _chequesRepository.Insert(
                        new Cheque()
                        {
                            IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                            IdTipoCheque = model.IdTipoCheque,
                            NroCheque = iLoop.ToString().PadLeft(10, '0'),
                            IdBanco = model.IdBanco,
                            Banco = banco?.Descripcion.ToUpper().Trim() ?? String.Empty,
                            Importe = 0,
                            IdEstadoCheque = (int)EstadoCheque.SinLibrar,
                            IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                            Firmante = String.Empty,
                            CUITFirmante = String.Empty,
                            Endosante1 = String.Empty,
                            CUITEndosante1 = String.Empty,
                            Endosante2 = String.Empty,
                            CUITEndosante2 = String.Empty,
                            Endosante3 = String.Empty,
                            CUITEndosante3 = String.Empty
                        },
                        tran
                     );
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();

            }
        }

        public async Task<ChequesViewModel> Get(int idCheque)
        {
            var cheque = await _chequesRepository.GetById<Cheque>(idCheque);

            if (cheque == null)
                return null;

            var model = _mapper.Map<ChequesViewModel>(cheque);

            return model;
        }

        public async Task<DataTablesResponse<Custom.Cheque>> GetAll(DataTablesRequest request)
        {
            var customQuery = _chequesRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("IdEstadoCheque"))
                builder.Append($"AND IdEstadoCheque = {filters["IdEstadoCheque"]}");

            if (filters.ContainsKey("IdTipoCheque"))
                builder.Append($"AND IdTipoCheque = {filters["IdTipoCheque"]}");

            if (filters.ContainsKey("NumeroCheque"))
                builder.Append($"AND NroCheque = {filters["NumeroCheque"]}");

            builder.Append($"AND IdEmpresa = {_permissionsBusiness.Value.User.IdEmpresa}");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Domain.EntitiesCustom.Cheque>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);            
        }

        public async Task Remove(int idCheque)
        {
            var cheque = await _chequesRepository.GetById<Cheque>(idCheque);

            if (cheque == null)
            {
                throw new BusinessException("Cheque inexistente");
            }

            cheque.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _chequesRepository.Update(cheque);
        }

        private void Validate(ChequesViewModel cheque)
        {
            if (cheque.NroDesde.IsNull())
            {
                throw new BusinessException($"Ingrese un Número de Cheque Desde");
            }

            if (cheque.NroHasta.IsNull())
            {
                throw new BusinessException($"Ingrese un Número de Cheque Hasta");
            }
        }

        public async Task<bool> ValidarNumeroCheque(int idBanco, int idTipoCheque, string nroDesde, string nroHasta)
            => await _chequesRepository.ValidarNumeroCheque(idBanco, idTipoCheque, nroDesde, nroHasta);

        public async Task<List<Domain.EntitiesCustom.ChequeTerceros>> GetChequesCartera(string q)
            => await _chequesRepository.GetChequesCartera(_permissionsBusiness.Value.User.IdEmpresa, q);

        public async Task<Cheque> GetPrimerChequeDisponible(int idBanco, int idTipoCheque)
            => await _chequesRepository.GetPrimerChequeDisponible(idBanco, idTipoCheque, _permissionsBusiness.Value.User.IdEmpresa);

        public async Task<bool> ValidarNumeroChequeDisponible(int idBanco, int idTipoCheque, string nroCheque)
            => await _chequesRepository.ValidarNumeroChequeDisponible(idBanco, idTipoCheque, nroCheque, _permissionsBusiness.Value.User.IdEmpresa);

        public async Task<List<ChequesADebitarViewModel>> GetChequesADebitar(int idBanco)
        {
            List<ChequesADebitarViewModel> chequesADebitarModel = new List<ChequesADebitarViewModel>();
            
            var chequesADebitar = await _chequesRepository.GetChequesADebitar(_permissionsBusiness.Value.User.IdEmpresa, idBanco);

            if (chequesADebitar != null && chequesADebitar.Count > 0)
            {
                foreach (var cheque in chequesADebitar)
                {
                    chequesADebitarModel.Add(new ChequesADebitarViewModel {
                        Seleccionar = false,
                        IdCheque = cheque.IdCheque,
                        NroCheque = cheque.NroCheque,
                        Fecha = cheque.FechaEmision.Value,
                        FechaDiferido = cheque.FechaVto.HasValue ? cheque.FechaVto.Value.ToString("dd/MM/yyyy") : string.Empty,
                        Importe = cheque.Importe
                    });
                }
            }

            return chequesADebitarModel;
        }

        public async Task Debitar(List<ChequesADebitarViewModel> model, int idEjercicio, DateTime fecha, int idBancoDebitar)
        {
            if (_ejerciciosRepository.EjercicioCerrado(idEjercicio, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Ejercicio seleccionado esta Cerrado. Verifique.");
            }

            if (_ejerciciosRepository.ValidateFechaEjercicio(idEjercicio, fecha, _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Fecha de los Cheques a Debitar no es Válida para el Ejercicio seleccionado. Verifique.");
            }

            if (_cierreMesRepository.MesCerrado(idEjercicio, fecha, Modulos.CajaBancos.Description(), _permissionsBusiness.Value.User.IdEmpresa).GetAwaiter().GetResult())
            {
                throw new BusinessException("El Mes seleccionado para los Cheques a Debitar esta Cerrado. Verifique.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                foreach (var cheque in model)
                {
                    if (cheque.Seleccionar)
                    { 
                        var chequeDB = await _chequesRepository.GetById<Cheque>(cheque.IdCheque, tran);
                        var banco = await _bancosRepository.GetById<Banco>(idBancoDebitar, tran);

                        #region Asiento Contable

                        var asiento = new Asiento
                        {
                            IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                            IdEjercicio = idEjercicio,
                            Descripcion = $"DEBITO CHEQUE NRO. {chequeDB.NroCheque}",
                            Fecha = fecha,
                            IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                            IdUsuario = _permissionsBusiness.Value.User.Id,
                            FechaIngreso = DateTime.Now
                        };

                        var idAsiento = await _asientosRepository.Insert(asiento, tran);

                        var indItem = 1;
                        var ctaChequesDiferidos = await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(_permissionsBusiness.Value.User.IdEmpresa, (int)CodigoObservacion.CHEQUES_DIFERIDOS, tran);
                        if (ctaChequesDiferidos != null)
                        {
                            await _asientosDetalleRepository.Insert(new AsientoDetalle
                            {
                                IdAsiento = (int)idAsiento,
                                Item = indItem++,
                                IdCuentaContable = ctaChequesDiferidos.IdCuentaContable,
                                Detalle = $"DEBITO CHEQUE NRO. {chequeDB.NroCheque}",
                                Debitos = cheque.Importe,
                                Creditos = 0
                            }, tran);
                        }
                        else
                            throw new BusinessException("No se ecnuentra la Cuenta Contable de Cheques Diferidos. Verifique.");

                        await _asientosDetalleRepository.Insert(new AsientoDetalle
                        {
                            IdAsiento = (int)idAsiento,
                            Item = indItem++,
                            IdCuentaContable = banco.IdCuentaContable,
                            Detalle = $"DEBITO CHEQUE NRO. {chequeDB.NroCheque}",
                            Debitos = 0,
                            Creditos = cheque.Importe
                        }, tran);


                        await _chequesDebitosAsientoRepository.Insert(new ChequeDebitoAsiento { IdCheque = cheque.IdCheque, IdAsiento = (int)idAsiento }, tran);

                        #endregion

                        chequeDB.IdEstadoCheque = (int)EstadoCheque.Debitado_Depositado;
                        await _chequesRepository.Update(chequeDB);
                    }
                }

                tran.Commit();
            }
            catch (BusinessException ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw ex;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id)
        {
            var asiento = await _chequesDebitosAsientoRepository.GetByIdCheque(id);
            if (asiento == null)
                return new List<Custom.AsientoDetalle>();

            return await _asientosDetalleRepository.GetAllByIdAsiento(asiento.IdAsiento);
        }

        public async Task ReverseById(int idCheque)
        {
            var cheque = await _chequesRepository.GetById<Cheque>(idCheque);

            if (cheque == null)
            {
                throw new BusinessException("Cheque Inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                var chequeAsiento = await _chequesDebitosAsientoRepository.GetByIdCheque(idCheque, tran);
                
                if(chequeAsiento == null)
                    throw new BusinessException("Asiento Inexistente");
                
                var asiento = await _asientosRepository.GetById<Asiento>(chequeAsiento.IdAsiento, tran);
                await _asientosDetalleRepository.DeleteByIdAsiento(chequeAsiento.IdAsiento, tran);
                await _chequesDebitosAsientoRepository.DeleteByIdCheque(idCheque, tran);

                asiento.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _asientosRepository.Update(asiento, tran);

                cheque.IdEstadoCheque = (int)EstadoCheque.Entregado;
                await _chequesRepository.Update<Cheque>(cheque, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task<IList<dynamic>> Detalle(int id)
        {
            var cheque = await _chequesRepository.GetById<Cheque>(id) ?? throw new BusinessException("Cheque Inexistente");

            List<dynamic> detalle = new();
            switch (cheque.IdTipoCheque)
            {
                case (int)TipoCheque.Terceros:
                    switch (cheque.IdEstadoCheque)
                    {
                        case (int)EstadoCheque.Rechazado:
                        case (int)EstadoCheque.EnCartera:
                            break;
                        case (int)EstadoCheque.Debitado_Depositado:
                            detalle = await _depositosBancoRepository.GetDepositoByCheque(cheque.IdCheque);
                            break;
                        case (int)EstadoCheque.Debitado_Rechazado:
                        case (int)EstadoCheque.Entregado:
                            detalle = await _ordenesPagoRepository.GetOrdenesPagoByCheque(cheque.IdCheque);
                            break;
                    }
                    break;
                default:
                    switch (cheque.IdEstadoCheque)
                    {
                        case (int)EstadoCheque.Entregado:
                        case (int)EstadoCheque.Librado:
                            detalle = await _ordenesPagoRepository.GetOrdenesPagoByCheque(cheque.IdCheque);
                            break;
                        case (int)EstadoCheque.SinLibrar:
                        case (int)EstadoCheque.Anulado:
                        case (int)EstadoCheque.Debitado_Depositado:
                        case (int)EstadoCheque.Rechazado:
                        case (int)EstadoCheque.Debitado_Rechazado:
                            break;
                    }
                    break;
            }

            return detalle;
        }
    }
}
