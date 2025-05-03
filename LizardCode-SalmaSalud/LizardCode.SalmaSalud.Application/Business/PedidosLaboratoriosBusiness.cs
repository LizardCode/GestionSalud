using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.Framework.Helpers.AFIP.Common;
using iTextSharp.text.pdf.codec.wmf;
using Microsoft.Extensions.Logging;
using Mysqlx.Cursor;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.PedidosLaboratorios;
using LizardCode.SalmaSalud.Application.Models.Presupuestos;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    internal class PedidosLaboratoriosBusiness : BaseBusiness, IPedidosLaboratoriosBusiness
    {
        private readonly ILogger<PedidosLaboratoriosBusiness> _logger;
        private readonly IPedidosLaboratoriosRepository _pedidosLaboratoriosRepository;
        private readonly IPedidosLaboratoriosServiciosRepository _pedidosLaboratoriosServiciosRepository;
        private readonly IPresupuestosRepository _presupuestosRepository;
        private readonly IPacientesRepository _pacientesRepository;
        private readonly ILaboratoriosServiciosRepository _laboratoriosServiciosRepository;
        private readonly IPedidosLaboratoriosHistorialRepository _pedidosLaboratoriosHistorialRepository;

        public PedidosLaboratoriosBusiness(
            IPedidosLaboratoriosRepository pedidosLaboratoriosRepository,
            IPedidosLaboratoriosServiciosRepository pedidosLaboratoriosServiciosRepository,
            IPresupuestosRepository PresupuestosRepository,
            ILogger<PedidosLaboratoriosBusiness> logger,
            IPacientesRepository pacientesRespository,
            ILaboratoriosServiciosRepository laboratoriosServiciosRepository,
            IPedidosLaboratoriosHistorialRepository pedidosLaboratoriosHistorialRepository)
        {
            _pedidosLaboratoriosRepository = pedidosLaboratoriosRepository;
            _pedidosLaboratoriosServiciosRepository = pedidosLaboratoriosServiciosRepository;
            _presupuestosRepository = PresupuestosRepository;
            _logger = logger;
            _pacientesRepository = pacientesRespository;
            _laboratoriosServiciosRepository = laboratoriosServiciosRepository;
            _pedidosLaboratoriosHistorialRepository = pedidosLaboratoriosHistorialRepository;
        }


        public async Task New(PedidoLaboratorioViewModel model)
        {
            var pedido = _mapper.Map<PedidoLaboratorio>(model);

            Validate(pedido);

            var presupuesto = await _presupuestosRepository.GetById<Presupuesto>(pedido.IdPresupuesto);
            if (presupuesto == null || presupuesto.IdEstadoPresupuesto != (int)EstadoPresupuesto.Aprobado)
            {
                throw new BusinessException("No se encontró el presupuesto.");
            }

            var paciente = await _pacientesRepository.GetById<Paciente>(presupuesto.IdPaciente);
            if (paciente == null)
            {
                throw new BusinessException("No se encontró el paciente.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                pedido.Fecha = DateTime.Now;

                pedido.IdPresupuesto = presupuesto.IdPresupuesto;
                pedido.IdLaboratorio = model.IdLaboratorio;

                pedido.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                pedido.IdUsuario = _permissionsBusiness.Value.User.Id;
                pedido.IdEstadoPedidoLaboratorio = (int)EstadoPedidoLaboratorio.Pendiente;
                pedido.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                pedido.IdFinanciador = paciente.IdFinanciador;
                pedido.IdFinanciadorPlan = paciente.IdFinanciadorPlan;
                pedido.FinanciadorNro = paciente.FinanciadorNro;
                pedido.Observaciones = model.Observaciones;

                var id = await _pedidosLaboratoriosRepository.Insert(pedido, tran);

                await UpdateServicios(id, model.Servicios, tran);

                await _pedidosLaboratoriosHistorialRepository.Insert(new PedidoLaboratorioHistorial()
                {
                    IdPedidoLaboratorio = (int)id,
                    Fecha = DateTime.Now,
                    FechaEstado = pedido.Fecha,
                    IdEstadoPedidoLaboratorio = pedido.IdEstadoPedidoLaboratorio,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    Observaciones = "Alta de Pedido a Laboratorio"
                }, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task<PedidoLaboratorioViewModel> Get(int idPedidoLaboratorio)
        {
            var pedido = await _pedidosLaboratoriosRepository.GetById<PedidoLaboratorio>(idPedidoLaboratorio);

            if (pedido == null)
                return null;

            var servicios = await _pedidosLaboratoriosServiciosRepository.GetAllByIdPedidoLaboratorio(idPedidoLaboratorio);

            var model = _mapper.Map<PedidoLaboratorioViewModel>(pedido);
            model.Servicios = _mapper.Map<List<PedidoLaboratorioServicioViewModel>>(servicios);

            return model;
        }

        public async Task<DataTablesResponse<Custom.PedidoLaboratorio>> GetAll(DataTablesRequest request)
        {
            var customQuery = _pedidosLaboratoriosRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("IdEstadoPedidoLaboratorio"))
                builder.Append($"AND IdEstadoPedidoLaboratorio = {filters["IdEstadoPedidoLaboratorio"]}");

            if (filters.ContainsKey("IdLaboratorioFilter"))
                builder.Append($"AND IdLaboratorio = {filters["IdLaboratorioFilter"]}");

            if (filters.ContainsKey("FiltroFechaDesde") && filters["FiltroFechaDesde"].ToString() != "__/__/____")
                builder.Append($"AND Fecha >= {DateTime.ParseExact(filters["FiltroFechaDesde"].ToString(), "dd/MM/yyyy", null)}");

            if (filters.ContainsKey("FiltroFechaHasta") && filters["FiltroFechaHasta"].ToString() != "__/__/____")
            {
                var date = DateTime.ParseExact(filters["FiltroFechaHasta"].ToString(), "dd/MM/yyyy", null);
                builder.Append($"AND Fecha <= {date.AddDays(1)}");
            }

            if (filters.ContainsKey("IdPacienteFilter"))
                builder.Append($"AND idPresupuesto IN (select idPresupuesto FROM Presupuestos WHERE IdPaciente = {filters["IdPacienteFilter"]}) ");

            if (filters.ContainsKey("IdPresupuesto")) {
                var idPresupuesto = 0;
                int.TryParse(filters["IdPresupuesto"].ToString(), out idPresupuesto);

                builder.Append($"AND IdPresupuesto = {idPresupuesto}");
            }

            builder.Append($"AND idPedidoLaboratorio IN (SELECT idPedidoLaboratorio FROM PedidosLaboratorios WHERE IdEmpresa IN (SELECT IdEmpresa FROM UsuariosEmpresas WHERE IdUsuario = {_permissionsBusiness.Value.User.Id}))");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.PedidoLaboratorio>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Remove(int idPedidoLaboratorio)
        {
            var pedido = await _pedidosLaboratoriosRepository.GetById<PedidoLaboratorio>(idPedidoLaboratorio);
            if (pedido == null)
            {
                throw new ArgumentException("Pedido inexistente");
            }

            if (pedido.IdEstadoPedidoLaboratorio != (int)EstadoPedidoLaboratorio.Pendiente)
            {
                throw new BusinessException("El Pedido no se encuentra en un estado válido");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                pedido.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _pedidosLaboratoriosRepository.Update(pedido, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        private void Validate(PedidoLaboratorio pedido)
        {
            if (pedido.IdPresupuesto == 0)
            {
                throw new BusinessException("Ingrese un Pedido válido.");
            }

            if (pedido.IdLaboratorio == 0)
            {
                throw new BusinessException("Ingrese un Laboratorio válido.");
            }
        }

        public async Task Marcar(EnviarItemViewModel model, EstadoPedidoLaboratorio estado)
        {
            var ids = new List<int>();
            foreach (var id in model.IdsPedidos.Split(","))
            {
                ids.Add(int.Parse(id));
            }

            var transaction = _uow.BeginTransaction();
            try
            {
                foreach (var idPedido in ids)
                {
                    if (estado == EstadoPedidoLaboratorio.Enviado) 
                    {
                        if (model.Fecha > DateTime.Now.Date)
                        {
                            throw new BusinessException("Fecha envío inválida.");
                        }

                        if (model.FechaEstimada < DateTime.Now.Date)
                        {
                            throw new BusinessException("Fecha estimada inválida.");
                        }

                        if (model.Fecha > model.FechaEstimada)
                        {
                            throw new BusinessException("Fecha Estimada inválida.");
                        }

                        await Enviar(idPedido, model.Fecha, model.FechaEstimada, model.NumeroSobre, model.Observaciones, transaction);
                    } 
                    else
                    {
                        await Recibir(idPedido, model.Fecha, model.Observaciones, transaction);
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                transaction.Rollback();
                throw new InternalException();
            }
        }

        public async Task Enviar(int idPedidoLaboratorio, DateTime fecha, DateTime fechaEstimada, int? numeroSobre, string? observaciones, IDbTransaction transaction)
        {
            var pedido = await _pedidosLaboratoriosRepository.GetById<PedidoLaboratorio>(idPedidoLaboratorio, transaction);
            if (pedido == null)
            {
                throw new BusinessException("Pedido inexistente.");
            }

            if (pedido.IdEmpresa != _permissionsBusiness.Value.User.IdEmpresa)
            {
                throw new BusinessException("No autorizado.");
            }

            if (pedido.IdEstadoPedidoLaboratorio != (int)EstadoPedidoLaboratorio.Pendiente)
            {
                throw new BusinessException("El pedido no se encuentra en un estado válido.");
            }

            try
            {
                pedido.IdEstadoPedidoLaboratorio = (int)EstadoPedidoLaboratorio.Enviado;
                pedido.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                pedido.FechaEnvio = fecha;
                pedido.FechaEstimada = fechaEstimada;
                pedido.NumeroSobre = numeroSobre;

                await _pedidosLaboratoriosRepository.Update(pedido, transaction);

                await _pedidosLaboratoriosHistorialRepository.Insert(new PedidoLaboratorioHistorial()
                {
                    IdPedidoLaboratorio = (int)idPedidoLaboratorio,
                    Fecha = DateTime.Now,
                    FechaEstado = fecha,
                    IdEstadoPedidoLaboratorio = pedido.IdEstadoPedidoLaboratorio,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    Observaciones = "Fecha Estimada: " + pedido.FechaEstimada?.ToString("dd/MM/yyyy") + Environment.NewLine
                                    + "Nro. Sobre: " + (numeroSobre.HasValue ? numeroSobre.ToString() : "N/D") + Environment.NewLine
                                    + observaciones
                }, transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Recibir(int idPedidoLaboratorio, DateTime fecha, string? observaciones, IDbTransaction transaction)
        {
            var pedido = await _pedidosLaboratoriosRepository.GetById<PedidoLaboratorio>(idPedidoLaboratorio, transaction);
            if (pedido == null)
            {
                throw new BusinessException("Pedido inexistente");
            }

            if (pedido.IdEmpresa != _permissionsBusiness.Value.User.IdEmpresa)
            {
                throw new BusinessException("No autorizado.");
            }

            if (pedido.IdEstadoPedidoLaboratorio != (int)EstadoPedidoLaboratorio.Enviado)
            {
                throw new BusinessException("El pedido no se encuentra en un estado válido.");
            }

            try
            {
                pedido.IdEstadoPedidoLaboratorio = (int)EstadoPedidoLaboratorio.Recibido;
                pedido.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                pedido.FechaRecepcion = fecha;

                await _pedidosLaboratoriosRepository.Update(pedido, transaction);

                await _pedidosLaboratoriosHistorialRepository.Insert(new PedidoLaboratorioHistorial()
                {
                    IdPedidoLaboratorio = (int)idPedidoLaboratorio,
                    Fecha = DateTime.Now,
                    FechaEstado = fecha,
                    IdEstadoPedidoLaboratorio = pedido.IdEstadoPedidoLaboratorio,
                    IdUsuario = _permissionsBusiness.Value.User.Id,
                    Observaciones = observaciones
                }, transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        private async Task UpdateServicios(long idPedidoLaboratorio, List<PedidoLaboratorioServicioViewModel> servicios, IDbTransaction transaction)
        {
            if (servicios != null && servicios.Count > 0)
            {
                var iItem = 1;
                foreach (var item in servicios)
                {
                    var servicio = await _laboratoriosServiciosRepository.GetById<LaboratorioServicio>(item.IdLaboratorioServicio, transaction);
                    await _pedidosLaboratoriosServiciosRepository.Insert(new PedidoLaboratorioServicio
                    {
                        IdPedidoLaboratorio = (int)idPedidoLaboratorio,
                        Item = iItem,
                        Servicio = servicio.Descripcion,
                        Valor = servicio.Valor

                    }, transaction);

                    iItem++;
                }
            }
        }

        public async Task<DataTablesResponse<Custom.PedidoLaboratorioHistorial>> GetHistorial(int idPedidoLaboratorio, DataTablesRequest request)
        {
            var customQuery = _pedidosLaboratoriosHistorialRepository.GetHistorial(idPedidoLaboratorio);
            var builder = _dbContext.Connection.QueryBuilder();

            return await _dataTablesService.Resolve<Custom.PedidoLaboratorioHistorial>(request, customQuery.Sql, customQuery.Parameters, staticWhere: builder.Sql);
        }
    }
}
