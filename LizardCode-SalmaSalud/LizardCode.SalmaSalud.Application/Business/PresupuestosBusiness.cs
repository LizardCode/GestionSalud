using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Common.Exceptions;
using Microsoft.Extensions.Logging;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Presupuestos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class PresupuestosBusiness : BaseBusiness, IPresupuestosBusiness
    {
        private readonly ILogger<PresupuestosBusiness> _logger;
        private readonly IPresupuestosRepository _presupuestosRepository;
        private readonly IPresupuestosPrestacionesRepository _presupuestosPrestacionesRepository;
        private readonly IPresupuestosOtrasPrestacionesRepository _presupuestosOtrasPrestacionesRepository;
        private readonly IPacientesRepository _pacientesRepository;

        private readonly IFinanciadoresPrestacionesRepository _financiadoresPrestacionesRepository;
        private readonly IPrestacionesRepository _prestacionesRepository;
        private readonly IPedidosLaboratoriosRepository _pedidosLaboratoriosRepository;


        public PresupuestosBusiness(
            IPresupuestosRepository PresupuestosRepository,
            ILogger<PresupuestosBusiness> logger,
            IPresupuestosPrestacionesRepository presupuestosPrestacionesRepository,
            IPresupuestosOtrasPrestacionesRepository presupuestosOtrasPrestacionesRepository,
            IPacientesRepository pacientesRespository,
            IFinanciadoresPrestacionesRepository financiadoresPrestacionesRepository,
            IPrestacionesRepository prestacionesRepository,
            IPedidosLaboratoriosRepository pedidosLaboratoriosRepository)
        {
            _presupuestosRepository = PresupuestosRepository;
            _logger = logger;
            _presupuestosPrestacionesRepository = presupuestosPrestacionesRepository;
            _presupuestosOtrasPrestacionesRepository = presupuestosOtrasPrestacionesRepository;
            _pacientesRepository = pacientesRespository;
            _financiadoresPrestacionesRepository = financiadoresPrestacionesRepository;
            _prestacionesRepository = prestacionesRepository;
            _pedidosLaboratoriosRepository = pedidosLaboratoriosRepository;
        }


        public async Task New(PresupuestoViewModel model)
        {
            var presupuesto = _mapper.Map<Presupuesto>(model);

            Validate(presupuesto);

            var paciente = await _pacientesRepository.GetById<Paciente>(presupuesto.IdPaciente);
            if (paciente == null)
            {
                throw new BusinessException("No se encontró el paciente.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                presupuesto.Fecha = DateTime.Now;
                presupuesto.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                presupuesto.IdUsuario = _permissionsBusiness.Value.User.Id;
                presupuesto.IdEstadoPresupuesto = (int)EstadoPresupuesto.Abierto;
                presupuesto.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                //presupuesto.IdPaciente = presupuesto.IdPaciente;
                presupuesto.IdFinanciador = paciente.IdFinanciador;
                presupuesto.IdFinanciadorPlan = paciente.IdFinanciadorPlan;
                presupuesto.FinanciadorNro = paciente.FinanciadorNro;

                var id = await _presupuestosRepository.Insert(presupuesto, tran);

                await UpdatePrestaciones(id, model.Prestaciones, tran);
                await UpdateOtrasPrestaciones(id, model.OtrasPrestaciones, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task<PresupuestoViewModel> Get(int idPresupuesto)
        {
            var presupuesto = await _presupuestosRepository.GetById<Presupuesto>(idPresupuesto);
            var prestaciones = await _presupuestosPrestacionesRepository.GetAllByIdPresupuesto(idPresupuesto);
            var otrasPrestaciones = await _presupuestosOtrasPrestacionesRepository.GetAllByIdPresupuesto(idPresupuesto);

            if (presupuesto == null)
                return null;

            var model = _mapper.Map<PresupuestoViewModel>(presupuesto);
            model.Prestaciones = _mapper.Map<List<PresupuestoPrestacionViewModel>>(prestaciones);
            model.OtrasPrestaciones = _mapper.Map<List<PresupuestoOtraPrestacionViewModel>>(otrasPrestaciones);

            return model;
        }

        public async Task<DataTablesResponse<Custom.Presupuesto>> GetAll(DataTablesRequest request)
        {
            var customQuery = _presupuestosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            //if (filters.ContainsKey("FiltroCUIT"))
            //    builder.Append($"AND CUIT = {filters["FiltroCUIT"]}");

            if (filters.ContainsKey("FiltroNombre"))
                builder.Append($"AND Paciente LIKE {string.Concat("%", filters["FiltroNombre"], "%")}");

            if (filters.ContainsKey("FiltroDocumento"))
                builder.Append($"AND PacienteDocumento LIKE {string.Concat("%", filters["FiltroDocumento"], "%")}");

            //builder.Append($"AND idPresupuesto IN (SELECT idPresupuesto FROM PresupuestosPlanes WHERE IdEmpresa IN (SELECT IdEmpresa FROM UsuariosEmpresas WHERE IdUsuario = {_permissionsBusiness.Value.User.Id}))");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Presupuesto>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(PresupuestoViewModel model)
        {
            var presupuesto = _mapper.Map<Presupuesto>(model);

            Validate(presupuesto);

            var paciente = await _pacientesRepository.GetById<Paciente>(presupuesto.IdPaciente);
            if (paciente == null)
            {
                throw new BusinessException("No se encontró el paciente.");
            }

            var dbPresupuesto = await _presupuestosRepository.GetById<Presupuesto>(presupuesto.IdPresupuesto);

            if (dbPresupuesto == null)
            {
                throw new ArgumentException("Presupuesto inexistente.");
            }

            if (dbPresupuesto.IdEstadoPresupuesto != (int)EstadoPresupuesto.Abierto)
            {
                throw new BusinessException("El presupuesto no se encuentra en un estado válido.");
            }


            var tran = _uow.BeginTransaction();

            try
            {
                dbPresupuesto.IdEstadoPresupuesto = (int)EstadoPresupuesto.Abierto;
                dbPresupuesto.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                dbPresupuesto.FechaVencimiento = model.FechaVencimiento;
                dbPresupuesto.IdFinanciador = paciente.IdFinanciador;
                dbPresupuesto.IdFinanciadorPlan = paciente.IdFinanciadorPlan;
                dbPresupuesto.FinanciadorNro = paciente.FinanciadorNro;
                dbPresupuesto.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                await _presupuestosRepository.Update(dbPresupuesto, tran);

                await UpdatePrestaciones(dbPresupuesto.IdPresupuesto, model.Prestaciones, tran);
                await UpdateOtrasPrestaciones(dbPresupuesto.IdPresupuesto, model.OtrasPrestaciones, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Remove(int idPresupuesto)
        {
            var presupuesto = await _presupuestosRepository.GetById<Presupuesto>(idPresupuesto);

            if (presupuesto == null)
            {
                throw new ArgumentException("Presupuesto inexistente");
            }

            var presupuestoEnPedido = await _presupuestosRepository.PresupuestoEnPedido(idPresupuesto);
            if (presupuestoEnPedido)
            {
                throw new ArgumentException("Presupuesto asociado a un Pedido a Laboratorio.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                presupuesto.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _presupuestosRepository.Update(presupuesto, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }


        private void Validate(Presupuesto presupuesto)
        {
            if (presupuesto.FechaVencimiento <= DateTime.Now.Date)
            {
                throw new BusinessException("Fecha de vencimiento de presupuesto inválida.");
            }

            if (presupuesto.IdPaciente == 0)
            {
                throw new BusinessException("Ingrese un paciente válido.");
            }
        }

        private async Task UpdatePrestaciones(long idPresupuesto, List<PresupuestoPrestacionViewModel> prestaciones, IDbTransaction transaction)
        {
            await _presupuestosPrestacionesRepository.RemoveByIdPresupuesto(idPresupuesto, transaction);

            if (prestaciones != null && prestaciones.Count > 0)
            {
                var iItem = 1;
                foreach (var item in prestaciones)
                {
                    var prestacion = await _financiadoresPrestacionesRepository.GetById<FinanciadorPrestacion>(item.IdPrestacion, transaction);
                    await _presupuestosPrestacionesRepository.Insert(new PresupuestoPrestacion
                    {
                        IdPresupuesto = (int)idPresupuesto,
                        Item = iItem,
                        IdPrestacion = prestacion.IdFinanciadorPrestacion,
                        Codigo = prestacion.Codigo,
                        Descripcion = prestacion.Descripcion,
                        Valor = item.Valor,
                        CoPago = item.CoPago

                    }, transaction);

                    iItem++;
                }
            }
        }
        public async Task<IList<PresupuestoPrestacion>> GetAllPrestacionesByPresupuestoId(int idPresupuesto)
            => await _presupuestosPrestacionesRepository.GetAllByIdPresupuesto(idPresupuesto);

        public async Task<IList<PresupuestoPrestacionViewModel>> GetAllPrestacionesByPresupuestoIds(List<int> ids)
        {
            var lPrestaciones = new List<PresupuestoPrestacionViewModel>();
            foreach (var id in ids)
            {
                var prestaciones = await _presupuestosPrestacionesRepository.GetAllByIdPresupuesto(id);
                lPrestaciones.AddRange(_mapper.Map<List<PresupuestoPrestacionViewModel>>(prestaciones));
            }

            return lPrestaciones;
        }        

        public async Task<PresupuestoPrestacion> GetPresupuestoPrestacionById(int idPresupuestoPrestacion)
            => await _presupuestosPrestacionesRepository.GetPresupuestoPrestacionById(idPresupuestoPrestacion);

        private async Task UpdateOtrasPrestaciones(long idPresupuesto, List<PresupuestoOtraPrestacionViewModel> prestaciones, IDbTransaction transaction)
        {
            await _presupuestosOtrasPrestacionesRepository.RemoveByIdPresupuesto(idPresupuesto, transaction);

            if (prestaciones != null && prestaciones.Count > 0)
            {
                var iItem = 1;
                foreach (var item in prestaciones)
                {
                    var prestacion = await _prestacionesRepository.GetById<Prestacion>(item.IdOtraPrestacion, transaction);
                    await _presupuestosOtrasPrestacionesRepository.Insert(new PresupuestoOtraPrestacion
                    {
                        IdPresupuesto = (int)idPresupuesto,
                        Item = iItem,
                        IdOtraPrestacion = prestacion.IdPrestacion,
                        Codigo = prestacion.Codigo,
                        Descripcion = prestacion.Descripcion,
                        Valor = item.Valor

                    }, transaction);

                    iItem++;
                }
            }
        }

        public async Task<IList<PresupuestoOtraPrestacion>> GetAllOtrasPrestacionesByPresupuestoId(int idPresupuesto)
            => await _presupuestosOtrasPrestacionesRepository.GetAllByIdPresupuesto(idPresupuesto);


        public async Task<IList<PresupuestoOtraPrestacionViewModel>> GetAllOtrasPrestacionesByPresupuestoIds(List<int> ids)
        {
            var lPrestaciones = new List<PresupuestoOtraPrestacionViewModel>();
            foreach (var id in ids)
            {
                var prestaciones = await _presupuestosOtrasPrestacionesRepository.GetAllByIdPresupuesto(id);
                lPrestaciones.AddRange(_mapper.Map<List<PresupuestoOtraPrestacionViewModel>>(prestaciones));
            }

            return lPrestaciones;
        }

        public async Task<PresupuestoOtraPrestacion> GetPresupuestoOtraPrestacionById(int idPresupuestoPrestacion)
            => await _presupuestosOtrasPrestacionesRepository.GetPresupuestoOtraPrestacionById(idPresupuestoPrestacion);

        public async Task Rechazar(int idPresupuesto)
        {
            var presupuesto = await _presupuestosRepository.GetById<Presupuesto>(idPresupuesto);
            if (presupuesto == null)
            {
                throw new BusinessException("Presupuesto inexistente");
            }

            if (presupuesto.FechaVencimiento < DateTime.Now.Date)
            {
                throw new BusinessException("La fecha del presupuesto es anterior a la fecha actual. El Presupuesto ya no puede ser rechazado.");
            }

            if (presupuesto.IdEstadoPresupuesto != (int)EstadoPresupuesto.Abierto)
            {
                throw new BusinessException("El presupuesto no se encuentra en un estado válido.");
            }

            try
            {
                presupuesto.IdEstadoPresupuesto = (int)EstadoPresupuesto.Rechazado;
                presupuesto.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                await _presupuestosRepository.Update(presupuesto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Aprobar(int idPresupuesto)
        {
            var presupuesto = await _presupuestosRepository.GetById<Presupuesto>(idPresupuesto);
            if (presupuesto == null)
            {
                throw new BusinessException("Presupuesto inexistente");
            }

            if (presupuesto.FechaVencimiento < DateTime.Now.Date)
            {
                throw new BusinessException("La fecha del presupuesto es anterior a la fecha actual. El Presupuesto ya no puede ser aprobado.");
            }

            if (presupuesto.IdEstadoPresupuesto != (int)EstadoPresupuesto.Abierto)
            {
                throw new BusinessException("El presupuesto no se encuentra en un estado válido.");
            }

            try
            {
                presupuesto.IdEstadoPresupuesto = (int)EstadoPresupuesto.Aprobado;
                presupuesto.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                await _presupuestosRepository.Update(presupuesto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Cerrar(int idPresupuesto)
        {
            var presupuesto = await _presupuestosRepository.GetById<Presupuesto>(idPresupuesto);
            if (presupuesto == null)
            {
                throw new BusinessException("Presupuesto inexistente");
            }

            if (presupuesto.IdEstadoPresupuesto != (int)EstadoPresupuesto.Aprobado)
            {
                throw new BusinessException("El presupuesto no se encuentra en un estado válido para ser cerrado.");
            }

            try
            {
                presupuesto.IdEstadoPresupuesto = (int)EstadoPresupuesto.Cerrado;
                presupuesto.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                await _presupuestosRepository.Update(presupuesto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task<List<Custom.Presupuesto>> GetPresupuestosAprobados(int idPaciente)
        {
            var presupuestos = await _presupuestosRepository.GetPresupuestosAprobados(_permissionsBusiness.Value.User.IdEmpresa, idPaciente);

            return presupuestos.ToList();
        }

        public async Task<List<Custom.Presupuesto>> GetPresupuestosAprobadosDisponibles()
        {
            var presupuestos = await _presupuestosRepository.GetPresupuestosAprobadosDisponibles(_permissionsBusiness.Value.User.IdEmpresa);

            return presupuestos.ToList();
        }

        public async Task<IList<Custom.PedidoLaboratorio>> GetPedidosPorPresupuesto(int idPresupuesto)
            => await _pedidosLaboratoriosRepository.GetPedidosPorPresupuesto(idPresupuesto);
    }
}
