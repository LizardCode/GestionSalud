using Dapper.DataTables.Models;
using DapperQueryBuilder;
using Microsoft.Extensions.Logging;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Evoluciones;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Threading.Tasks;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Helpers.Utilities;
using System.Linq;
using System.Collections.Generic;
using LizardCode.SalmaSalud.Application.Interfaces.Services;
using LizardCode.SalmaSalud.Domain.EntitiesCustom.Blockchain;
using Newtonsoft.Json;
using LizardCode.SalmaSalud.Application.Models.Evoluciones.Odontograma;
using System.IO;
using LizardCode.SalmaSalud.Application.Models.Reportes;
using LizardCode.Framework.Application.Common.Enums;
using System.Data;
using LizardCode.Framework.Helpers.Excel;
using Microsoft.AspNetCore.Http;
using static iTextSharp.text.pdf.AcroFields;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class EvolucionesBusiness : BaseBusiness, IEvolucionesBusiness
    {
        private List<string> _allowedImageTypes = new List<string> { "image/jpeg", "image/gif", "image/png" };

        private readonly ILogger<EmpresasBusiness> _logger;
        private readonly IEvolucionesRepository _evolucionesRepository;
        private readonly IEvolucionesPrestacionesRepository _evolucionesPrestacionesRepository;
        private readonly IEvolucionesOtrasPrestacionesRepository _evolucionesOtrasPrestacionesRepository;
		private readonly IEvolucionesOdontogramasPiezasRepository _evolucionesOdontogramasPiezasRepository;
		private readonly IEvolucionesOdontogramasPiezasZonasRepository _evolucionesOdontogramasPiezasZonasRepository;
        private readonly IEvolucionesArchivosRepository _evolucionesArchivosRepository;
        private readonly IEvolucionesRecetasRepository _evolucionesRecetasRepository;
        private readonly IEvolucionesOrdenesRepository _evolucionesOrdenesRepository;

        private readonly ITurnosRepository _turnosRepository;
        private readonly ITurnosHistorialRepository _turnosHistorialRepository;
        private readonly IPrestacionesRepository _prestacionesRepository;
        private readonly IFinanciadoresPrestacionesRepository _financiadorPrestacionesRepository;
        private readonly IPacientesRepository _pacientesRepository;
        private readonly IArchivosRepository _archivosRepository;
        private readonly IVademecumRepository _vademecumRepository;

        private readonly IBlockchainService _blockchainService;
        private readonly IMailBusiness _mailBusiness;
        private readonly IImpresionesBusiness _impresionesBusiness;
        private readonly IPresupuestosRepository _presupuestosRepository;

        private readonly IPrestacionesProfesionalesRepository _prestacionesProfesionalesRepository;
        private readonly IFinanciadoresPrestacionesProfesionalesRepository _financiadoresPrestacionesProfesionalesRepository;

        private readonly IClientesRepository _clientesRepository;
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly IFinanciadoresPlanesRepository _financiadoresPlanesRepository;
        private readonly IProfesionalesRepository _profesionalesRepository;

        public EvolucionesBusiness(ILogger<EmpresasBusiness> logger,
                                    IEvolucionesRepository evolucionesRepository,
                                    IEvolucionesPrestacionesRepository evolucionesPrestacionesRepository,
                                    IEvolucionesOtrasPrestacionesRepository evolucionesOtrasPrestacionesRepository,
									IEvolucionesOdontogramasPiezasRepository evolucionesOdontogramasPiezasRepository,
									IEvolucionesOdontogramasPiezasZonasRepository evolucionesOdontogramasPiezasZonasRepository,
                                    IEvolucionesArchivosRepository evolucionesArchivosRepository,
                                    IEvolucionesRecetasRepository evolucionesRecetasRepository,
                                    IEvolucionesOrdenesRepository evolucionesOrdenesRepository,
                                    ITurnosRepository turnosRepository,
                                    ITurnosHistorialRepository turnosHistorialRepository,
                                    IPrestacionesRepository prestacionesRepository,
                                    IFinanciadoresPrestacionesRepository financiadorPrestacionesRepository,
                                    IPacientesRepository pacientesRepository,
                                    IArchivosRepository archivosRepository,
                                    IVademecumRepository vademecumRepository,
                                    IBlockchainService blockchainService,
                                    IMailBusiness mailBusiness,
                                    IImpresionesBusiness impresionesBusiness, 
                                    IPresupuestosRepository presupuestosRepository,
                                    IPrestacionesProfesionalesRepository prestacionesProfesionalesRepository,
                                    IFinanciadoresPrestacionesProfesionalesRepository financiadoresPrestacionesProfesionalesRepository,
                                    IClientesRepository clientesRepository,
                                    IUsuariosRepository usuariosRepository,
                                    IFinanciadoresPlanesRepository financiadoresPlanesRepository,
                                    IProfesionalesRepository profesionalesRepository)
        {
            _logger = logger;
            _evolucionesRepository = evolucionesRepository;
            _evolucionesPrestacionesRepository = evolucionesPrestacionesRepository;
            _evolucionesOtrasPrestacionesRepository = evolucionesOtrasPrestacionesRepository;
			_evolucionesOdontogramasPiezasRepository = evolucionesOdontogramasPiezasRepository;
            _evolucionesOdontogramasPiezasZonasRepository = evolucionesOdontogramasPiezasZonasRepository;
            _evolucionesArchivosRepository = evolucionesArchivosRepository;
            _evolucionesRecetasRepository = evolucionesRecetasRepository;
            _evolucionesOrdenesRepository = evolucionesOrdenesRepository;

			_turnosRepository = turnosRepository;
            _turnosHistorialRepository = turnosHistorialRepository;
            _prestacionesRepository = prestacionesRepository;
            _financiadorPrestacionesRepository = financiadorPrestacionesRepository;
            _pacientesRepository = pacientesRepository;
            _archivosRepository = archivosRepository;
            _vademecumRepository = vademecumRepository;
            _blockchainService = blockchainService;
            _mailBusiness = mailBusiness;
            _impresionesBusiness = impresionesBusiness;
            _presupuestosRepository = presupuestosRepository;

            _prestacionesProfesionalesRepository = prestacionesProfesionalesRepository;
            _financiadoresPrestacionesProfesionalesRepository = financiadoresPrestacionesProfesionalesRepository;

            _clientesRepository = clientesRepository;
            _usuariosRepository = usuariosRepository;

            _financiadoresPlanesRepository = financiadoresPlanesRepository;
            _profesionalesRepository = profesionalesRepository;
        }
        public async Task<Custom.Evolucion> GetCustomById(int idEvolucion)
            => await _evolucionesRepository.GetCustomById(idEvolucion);

        public async Task<EvolucionViewModel> Get(int idEvolucion)
        {
            var evolucion = await _evolucionesRepository.GetById<Evolucion>(idEvolucion);
            //var empresas = await _ProfesionalesEmpresasRepository.GetAllByIdProfesional(idProfesional);

            if (evolucion == null)
                return null;

            var model = _mapper.Map<EvolucionViewModel>(evolucion);
            //model.Empresas = empresas.Select(e => e?.IdEmpresa).ToList();

            return model;
        }

        public async Task<DataTablesResponse<Custom.Evolucion>> GetAll(DataTablesRequest request, int idPaciente = 0)
        {
            var customQuery = _evolucionesRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            //if (filters.ContainsKey("FiltroNombre"))
            //    builder.Append($"AND Nombre LIKE {string.Concat("%", filters["FiltroNombre"], "%")}");

            if (_permissionsBusiness.Value.User.IdPaciente > 0)
            {
                builder.Append($"AND idPaciente = {_permissionsBusiness.Value.User.IdPaciente} ");
            }
            else 
            {
                if (idPaciente > 0)
                    builder.Append($"AND idPaciente = {idPaciente} ");
            }
                

            if (_permissionsBusiness.Value.User.IdEmpresa > 0)
                builder.Append($"AND idEmpresa = {_permissionsBusiness.Value.User.IdEmpresa} ");

            builder.Append($"AND idEstadoRegistro != {(int)EstadoRegistro.Eliminado} ");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Evolucion>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task New(EvolucionViewModel model)
        {
            var evolucion = _mapper.Map<Evolucion>(model);
            var prestaciones = model.Prestaciones;
            var otrasPrestaciones = model.OtrasPrestaciones;
			var piezas = model.Piezas;

			Validate(evolucion);

            if (model.Imagenes != null && model.Imagenes.Count > 6)
            {
                throw new BusinessException("Solo puede subir hasta 6 imágenes.");
            }

            var turno = await _turnosRepository.GetById<Turno>(model.IdTurno);
            if (turno == null)
                throw new BusinessException("No se encontró el turno que originó la evolución.");

            //Validaciones de pertennecia al profesional  y empresa
            if (turno.IdTipoTurno == (int)TipoTurno.Guardia)
            {
                if (turno.IdEmpresa != _permissionsBusiness.Value.User.IdEmpresa)
                    throw new BusinessException("Profesional no autorizado.");
            }
            else
            { 
                if (turno.IdEmpresa != _permissionsBusiness.Value.User.IdEmpresa
                    || turno.IdProfesional != _permissionsBusiness.Value.User.IdProfesional)
                    throw new BusinessException("Profesional no autorizado.");
            }

            var profesional = (await _lookupsBusiness.Value.GetAllProfesionales(_permissionsBusiness.Value.User.IdEmpresa)).FirstOrDefault(f => f.IdProfesional == _permissionsBusiness.Value.User.IdProfesional);
            var paciente = await _pacientesRepository.GetById<Paciente>(turno.IdPaciente);

            var tran = _uow.BeginTransaction();

            var idEvolucion = 0;
            var recetasAImprimir = new List<int>();
            var ordenesAImprimir = new List<int>();
            try
            {
                paciente.UltimaAtencion = DateTime.Now;
                await _pacientesRepository.Update(paciente, tran);

                evolucion.IdTurno = turno.IdTurno;
                evolucion.IdPaciente = turno.IdPaciente;

                evolucion.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                evolucion.IdProfesional = _permissionsBusiness.Value.User.IdProfesional;
                evolucion.IdEspecialidad = profesional.IdEspecialidad;

                if (!turno.ForzarParticular)
                { 
                    evolucion.IdFinanciador = paciente.IdFinanciador;
                    evolucion.IdFinanciadorPlan = paciente.IdFinanciadorPlan;
                    evolucion.FinanciadorNro = paciente.FinanciadorNro;
                }

                evolucion.Fecha = DateTime.Now;
                evolucion.Diagnostico = model.Diagnostico;
                evolucion.Observaciones = model.Observaciones;

                evolucion.IdEstadoEvolucion = (int)EstadoEvolucion.Realizada;

                var id = await _evolucionesRepository.Insert(evolucion, tran);
                idEvolucion = (int)id;

                //Blockchain
                 await _blockchainService.AddBlock(new Block {Data = JsonConvert.SerializeObject(evolucion) },tran);

                //Prestaciones
                if (prestaciones != null && prestaciones.Count > 0)
                {
                    int i = 1;
                    foreach (var p in prestaciones)
                    {
                        var prestacion = await _financiadorPrestacionesRepository.GetFinanciadorPrestacionById(p.IdPrestacion, tran);
                        var prestacionProfesional = await _financiadoresPrestacionesProfesionalesRepository.GetByIdPrestacionAndProfesional(p.IdPrestacion, evolucion.IdProfesional, tran);

                        var valorFijo = prestacion.ValorFijo ?? 0;
                        if (prestacionProfesional != null)
                            valorFijo = prestacionProfesional.ValorFijo ?? 0;

                        var porcentaje = prestacion.Porcentaje ?? 0; 
                        if (prestacionProfesional != null)
                            porcentaje = prestacionProfesional.Porcentaje ?? 0;

                        var idEvolucionPrestacion = await _evolucionesPrestacionesRepository.Insert(new EvolucionPrestacion() { 
                            IdEvolucion = (int)id, 
                            Item = i,
                            Pieza = p.Pieza,
                            Descripcion = prestacion.Descripcion,
                            Valor = prestacion.Valor, // - p.CoPago, 
                            IdEstadoPrestacion = (int)EstadoPrestacion.Realizada,
                            IdTipoPrestacion = (int)TipoPrestacion.Prestacion,
                            Codigo = prestacion.Codigo,

                            IdPrestacion = prestacion.IdPrestacion,
                            CodigoPrestacion = prestacion.CodigoPrestacion,

                            ValorFijo = valorFijo,
                            Porcentaje = porcentaje
                        }, tran);

                        if (prestacion.CoPago > 0)
                        {
                            i++;

                            await _evolucionesPrestacionesRepository.Insert(new EvolucionPrestacion()
                            {
                                IdEvolucion = (int)id,
                                Item = i,
                                Pieza = p.Pieza,
                                Codigo = prestacion.Codigo,
                                Descripcion = prestacion.Descripcion,
                                Valor = prestacion.CoPago.Value,
                                IdEstadoPrestacion = (int)EstadoPrestacion.Realizada,
                                IdTipoPrestacion = (int)TipoPrestacion.CoPago,

                                IdEvolucionPrestacionAsociada = idEvolucionPrestacion
                            }, tran);
                        }

                        i++;
                    }
                }

                //Otras Prestaciones
                if (otrasPrestaciones != null && otrasPrestaciones.Count > 0)
                {
                    int i = 1;
                    foreach (var p in otrasPrestaciones)
                    {
                        var prestacion = await _prestacionesRepository.GetById<Prestacion>(p.IdOtraPrestacion, tran);
                        var prestacionProfesional = await _prestacionesProfesionalesRepository.GetByIdPrestacionAndProfesional(p.IdOtraPrestacion, evolucion.IdProfesional, tran);

                        var valorFijo = prestacion.ValorFijo ?? 0;
                        if (prestacionProfesional != null)
                            valorFijo = prestacionProfesional.ValorFijo ?? 0;

                        var porcentaje = prestacion.Porcentaje ?? 0;
                        if (prestacionProfesional != null)
                            porcentaje = prestacionProfesional.Porcentaje ?? 0;

                        await _evolucionesOtrasPrestacionesRepository.Insert(new EvolucionOtraPrestacion()
                        {
                            IdEvolucion = (int)id,
                            Item = i,
                            Pieza = p.Pieza,
                            Codigo = prestacion.Codigo,
                            Descripcion = prestacion.Descripcion,
                            Valor = prestacion.Valor,
                            IdEstadoPrestacion = (int)EstadoPrestacion.Realizada,

                            ValorFijo = valorFijo,
                            Porcentaje = porcentaje
                        }, tran);

                        i++;
                    }
                }

                //Odontograma
                if (piezas != null && piezas.Count > 0)
                {
                    foreach (var pieza in piezas)
                    {
						var idOP = await _evolucionesOdontogramasPiezasRepository.Insert(new EvolucionOdontogramaPieza()
						{
							IdEvolucion = (int)id,
							Pieza = pieza.Pieza,
							Caries = pieza.Caries,
							Corona = pieza.Corona,
							PrFija = pieza.PrFija,
							PrRemovible = pieza.PrRemovible,
							Amalgama = pieza.Amalgama,
							Ausente = pieza.Ausente,
							Ortodoncia = pieza.Ortodoncia,
							Extraccion = pieza.Extraccion,
                            Observaciones = pieza.Observaciones
						}, tran);

                        foreach (var zona in pieza.Zonas)
                        {
							await _evolucionesOdontogramasPiezasZonasRepository.Insert(new EvolucionOdontogramaPiezaZona()
							{
								IdEvolucionOdontogramaPieza = (int)idOP,
                                IdEvolucion = (int)id,
								Pieza = pieza.Pieza,
								Zona = zona.Zona,
								IdTipoTrabajoOdontograma = zona.TipoTrabajo
							}, tran);
						}
					}
                }

                //Recetas
                if (model.Recetas != null && model.Recetas.Count > 0)
                {
                    foreach (var receta in model.Recetas)
                    {
                        var vademecum = await _vademecumRepository.GetById<Vademecum>(receta.IdVademecum, tran);
                        if (vademecum != null) { 
                            var idEvolucionReceta = await _evolucionesRecetasRepository.Insert(new EvolucionReceta
                            {
                                IdEvolucion = (int)id,
                                Fecha = DateTime.Now,
                                IdVademecum = vademecum.IdVademecum,
                                Descripcion = string.Format("{0} {1} - ({2})", vademecum.PrincipioActivo, vademecum.Potencia, vademecum.NombreComercial), //receta.Descripcion,
                                Cantidad = receta.Cantidad,
                                Dosis = receta.Dosis,
                                Frecuencia = receta.Frecuencia,
                                Indicaciones = receta.Indicaciones,

                                Codigo = vademecum.Codigo,
                                CodigoTroquel = vademecum.CodigoTroquel,
                                PrincipioActivo = vademecum.PrincipioActivo,
                                NombreComercial = vademecum.NombreComercial,
                                Potencia = vademecum.Potencia,
                                Contenido = vademecum.Contenido,
                                FormaFarmaceutica = vademecum.FormaFarmaceutica,
                                Laboratorio = vademecum.Laboratorio,
                                Observaciones = vademecum.Observaciones,

                                NoSustituir = receta.NoSustituir
                            }, tran);

                            recetasAImprimir.Add((int)idEvolucionReceta);
                        }
                    }
                }

                //Órdenes
                if (model.Ordenes != null && model.Ordenes.Count > 0)
                {
                    foreach (var orden in model.Ordenes)
                    {
                        var idEvolucionOrden = await _evolucionesOrdenesRepository.Insert(new EvolucionOrden
                        {
                            IdEvolucion = (int)id,
                            Fecha = DateTime.Now,
                            Descripcion = orden.Descripcion,                            
                            Indicaciones = orden.Indicaciones
                        }, tran);

                        ordenesAImprimir.Add((int)idEvolucionOrden);
                    }
                }

                //Imagenes
                if (model.Imagenes != null && model.Imagenes.Count > 0)
                {
                    foreach (var imagen in model.Imagenes)
                    {
                        using (var ms = new MemoryStream())
                        {
                            imagen.CopyTo(ms);
                            var fileBytes = ms.ToArray();

                            var fileSignature = new FileSignature(fileBytes);
                            var file = fileSignature.Parse();

                            if (file != null)
                            {
                                if (!_allowedImageTypes.Contains(file.MimeType.ToLowerInvariant()))
                                {
                                    throw new BusinessException("Tipo de imagen inválida.");
                                }

                                var idArchivo = await _archivosRepository.Insert(new Archivo
                                {
                                    Fecha = DateTime.Now,
                                    Nombre = imagen.FileName,
                                    Tipo = file.MimeType,
                                    Extension= file.Extension,
                                    Contenido = fileBytes
                                }, tran);

                                await _evolucionesArchivosRepository.Insert(new EvolucionArchivo
                                {
                                    IdArchivo = (int)idArchivo,
                                    IdEvolucion = (int)id
                                }, tran);
                            }
                            else
                            {
                                throw new BusinessException("Una o mas imágenes inválidas.");
                            }
                        }
                    }
                }

                //Actualizo presupuestos
                if (!string.IsNullOrEmpty(model.Presupuestos))
                {
                    var lPresupuestos = model.Presupuestos.Split(",");
                    if (lPresupuestos.Length > 0)
                    {
                        foreach (var lP in lPresupuestos)
                        {
                            var presupuesto = await _presupuestosRepository.GetById<Presupuesto>(int.Parse(lP), tran);
                            presupuesto.IdEvolucion = (int)id;
                            await _presupuestosRepository.Update(presupuesto, tran);
                        }
                    }
                }

				//Actualizo el turno
				turno.IdEstadoTurno = (int)EstadoTurno.Atendido;
                await _turnosRepository.Update(turno, tran);

                //Agrego historial
                await _turnosHistorialRepository.Insert(new TurnoHistorial
                {
                    Fecha = DateTime.Now,
                    IdEstadoTurno = (int)EstadoTurno.Atendido,
                    IdTurno = turno.IdTurno,
                    IdUsuario= _permissionsBusiness.Value.User.Id,
                    Observaciones = string.Empty
                }, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new BusinessException(ex.Message);
            }

            if (recetasAImprimir.Count > 0 || ordenesAImprimir.Count > 0)
            {
                var recetas = new Dictionary<string, string>();
                foreach (var idEvolucionReceta in recetasAImprimir)
                {
                    var pdf = await _impresionesBusiness.GenerarImpresionReceta(idEvolucionReceta, idEvolucion);
                    var sPdf = Convert.ToBase64String(pdf.Content);

                    recetas.Add($"{idEvolucionReceta}.pdf", sPdf);
                }

                foreach (var idEvolucionOrden in ordenesAImprimir)
                {
                    var pdf = await _impresionesBusiness.GenerarImpresionOrden(idEvolucionOrden, idEvolucion);
                    var sPdf = Convert.ToBase64String(pdf.Content);

                    recetas.Add($"{idEvolucionOrden}.pdf", sPdf);
                }

                //var pdfIndicaciones = await _impresionesBusiness.GenerarImpresionReceta(0, idEvolucion);
                //var sPdfIndicaciones = Convert.ToBase64String(pdfIndicaciones.Content);

                //recetas.Add(pdfIndicaciones.Filename, sPdfIndicaciones);

                _mailBusiness.EnviarMailRecetasPaciente(paciente.Email, paciente.Nombre, recetas);
            }
        }

        public async Task NewExterna(EvolucionExternaViewModel model, IDbTransaction transaction = null)
        {
            var evolucion = _mapper.Map<Evolucion>(model);
            var prestaciones = model.Prestaciones;
            var otrasPrestaciones = model.OtrasPrestaciones;
            var piezas = model.Piezas;

            Validate(evolucion);

            if (model.Imagenes != null && model.Imagenes.Count > 6)
            {
                throw new BusinessException("Solo puede subir hasta 6 imágenes.");
            }
                        
            //var profesional = (await _lookupsBusiness.Value.GetAllProfesionales(_permissionsBusiness.Value.User.IdEmpresa)).FirstOrDefault(f => f.IdProfesional == _permissionsBusiness.Value.User.IdProfesional);
            var profesional = await _profesionalesRepository.GetById<Profesional>(_permissionsBusiness.Value.User.IdProfesional, transaction);
            //var paciente = await _pacientesRepository.GetById<Paciente>(turno.IdPaciente);
            Paciente paciente = null;
            long idPaciente = 0;
            if (model.IdPaciente > 0)
            {
                paciente = await _pacientesRepository.GetById<Paciente>(model.IdPaciente, transaction);
                if (paciente == null)
                {
                    throw new BusinessException("No se encontró el paciente.");
                }
            }

            var commitTranHere = transaction == null;

            var tran = transaction ?? _uow.BeginTransaction();

            var idEvolucion = 0;
            var recetasAImprimir = new List<int>();
            var ordenesAImprimir = new List<int>();
            try
            {
                //long idPaciente = 0;
                if (model.IdPaciente == 0)
                {
                    //Creo un paciente nuevo
                    paciente = new Paciente
                    {
                        Nombre = model.Nombre.ToUpperInvariant(),
                        Documento = model.Documento,
                        Email = model.Email,
                        IdTipoTelefono = (int)TipoTelefono.Movil, //model.IdTipoTelefono,
                        Telefono = model.Telefono,
                        IdFinanciador = model.SinCobertura ? null : model.IdFinanciador,
                        IdFinanciadorPlan = model.SinCobertura ? null : model.IdFinanciadorPlan,
                        FinanciadorNro = model.SinCobertura ? null : model.FinanciadorNro,
                        FechaNacimiento = model.FechaNacimiento,
                        Nacionalidad = model.Nacionalidad,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo

                    };

                    idPaciente = await NewPacienteCliente(paciente, tran);
                }
                else
                {
                    idPaciente = model.IdPaciente;
                }

                paciente.UltimaAtencion = DateTime.Now;
                await _pacientesRepository.Update(paciente, tran);

                evolucion.IdTurno = null;
                evolucion.IdPaciente = (Int32)idPaciente;

                evolucion.IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa;
                evolucion.IdProfesional = _permissionsBusiness.Value.User.IdProfesional;
                evolucion.IdEspecialidad = profesional.IdEspecialidad;

                //if (!turno.ForzarParticular)
                //{
                evolucion.IdFinanciador = paciente.IdFinanciador;
                evolucion.IdFinanciadorPlan = paciente.IdFinanciadorPlan;
                evolucion.FinanciadorNro = paciente.FinanciadorNro;
                //}

                evolucion.Fecha = model.Fecha;
                evolucion.Diagnostico = model.Diagnostico;
                evolucion.Observaciones = model.Observaciones;

                evolucion.IdEstadoEvolucion = (int)EstadoEvolucion.Realizada;

                var id = await _evolucionesRepository.Insert(evolucion, tran);
                idEvolucion = (int)id;

                //Blockchain
                await _blockchainService.AddBlock(new Block { Data = JsonConvert.SerializeObject(evolucion) }, tran);

                //Prestaciones
                if (prestaciones != null && prestaciones.Count > 0)
                {
                    int i = 1;
                    foreach (var p in prestaciones)
                    {
                        var prestacion = await _financiadorPrestacionesRepository.GetFinanciadorPrestacionById(p.IdPrestacion, tran);
                        var prestacionProfesional = await _financiadoresPrestacionesProfesionalesRepository.GetByIdPrestacionAndProfesional(p.IdPrestacion, evolucion.IdProfesional, tran);

                        var valorFijo = prestacion.ValorFijo ?? 0;
                        if (prestacionProfesional != null)
                            valorFijo = prestacionProfesional.ValorFijo ?? 0;

                        var porcentaje = prestacion.Porcentaje ?? 0;
                        if (prestacionProfesional != null)
                            porcentaje = prestacionProfesional.Porcentaje ?? 0;

                        var idEvolucionPrestacion = await _evolucionesPrestacionesRepository.Insert(new EvolucionPrestacion()
                        {
                            IdEvolucion = (int)id,
                            Item = i,
                            Pieza = p.Pieza,
                            Descripcion = prestacion.Descripcion,
                            Valor = prestacion.Valor, // - p.CoPago, 
                            IdEstadoPrestacion = (int)EstadoPrestacion.Realizada,
                            IdTipoPrestacion = (int)TipoPrestacion.Prestacion,
                            Codigo = prestacion.Codigo,

                            IdPrestacion = prestacion.IdPrestacion,
                            CodigoPrestacion = prestacion.CodigoPrestacion,

                            ValorFijo = valorFijo,
                            Porcentaje = porcentaje
                        }, tran);

                        if (prestacion.CoPago > 0)
                        {
                            i++;

                            await _evolucionesPrestacionesRepository.Insert(new EvolucionPrestacion()
                            {
                                IdEvolucion = (int)id,
                                Item = i,
                                Pieza = p.Pieza,
                                Codigo = prestacion.Codigo,
                                Descripcion = prestacion.Descripcion,
                                Valor = prestacion.CoPago.Value,
                                IdEstadoPrestacion = (int)EstadoPrestacion.Realizada,
                                IdTipoPrestacion = (int)TipoPrestacion.CoPago,

                                IdEvolucionPrestacionAsociada = idEvolucionPrestacion
                            }, tran);
                        }

                        i++;
                    }
                }

                //Otras Prestaciones
                if (otrasPrestaciones != null && otrasPrestaciones.Count > 0)
                {
                    int i = 1;
                    foreach (var p in otrasPrestaciones)
                    {
                        var prestacion = await _prestacionesRepository.GetById<Prestacion>(p.IdOtraPrestacion, tran);
                        var prestacionProfesional = await _prestacionesProfesionalesRepository.GetByIdPrestacionAndProfesional(p.IdOtraPrestacion, evolucion.IdProfesional, tran);

                        var valorFijo = prestacion.ValorFijo ?? 0;
                        if (prestacionProfesional != null)
                            valorFijo = prestacionProfesional.ValorFijo ?? 0;

                        var porcentaje = prestacion.Porcentaje ?? 0;
                        if (prestacionProfesional != null)
                            porcentaje = prestacionProfesional.Porcentaje ?? 0;

                        await _evolucionesOtrasPrestacionesRepository.Insert(new EvolucionOtraPrestacion()
                        {
                            IdEvolucion = (int)id,
                            Item = i,
                            Pieza = p.Pieza,
                            Codigo = prestacion.Codigo,
                            Descripcion = prestacion.Descripcion,
                            Valor = prestacion.Valor,
                            IdEstadoPrestacion = (int)EstadoPrestacion.Realizada,

                            ValorFijo = valorFijo,
                            Porcentaje = porcentaje
                        }, tran);

                        i++;
                    }
                }

                //Odontograma
                if (piezas != null && piezas.Count > 0)
                {
                    foreach (var pieza in piezas)
                    {
                        var idOP = await _evolucionesOdontogramasPiezasRepository.Insert(new EvolucionOdontogramaPieza()
                        {
                            IdEvolucion = (int)id,
                            Pieza = pieza.Pieza,
                            Caries = pieza.Caries,
                            Corona = pieza.Corona,
                            PrFija = pieza.PrFija,
                            PrRemovible = pieza.PrRemovible,
                            Amalgama = pieza.Amalgama,
                            Ausente = pieza.Ausente,
                            Ortodoncia = pieza.Ortodoncia,
                            Extraccion = pieza.Extraccion,
                            Observaciones = pieza.Observaciones
                        }, tran);

                        foreach (var zona in pieza.Zonas)
                        {
                            await _evolucionesOdontogramasPiezasZonasRepository.Insert(new EvolucionOdontogramaPiezaZona()
                            {
                                IdEvolucionOdontogramaPieza = (int)idOP,
                                IdEvolucion = (int)id,
                                Pieza = pieza.Pieza,
                                Zona = zona.Zona,
                                IdTipoTrabajoOdontograma = zona.TipoTrabajo
                            }, tran);
                        }
                    }
                }

                //Recetas
                if (model.Recetas != null && model.Recetas.Count > 0)
                {
                    foreach (var receta in model.Recetas)
                    {
                        var vademecum = await _vademecumRepository.GetById<Vademecum>(receta.IdVademecum, tran);
                        if (vademecum != null)
                        {
                            var idEvolucionReceta = await _evolucionesRecetasRepository.Insert(new EvolucionReceta
                            {
                                IdEvolucion = (int)id,
                                Fecha = DateTime.Now,
                                IdVademecum = vademecum.IdVademecum,
                                Descripcion = string.Format("{0} {1} - ({2})", vademecum.PrincipioActivo, vademecum.Potencia, vademecum.NombreComercial), //receta.Descripcion,
                                Cantidad = receta.Cantidad,
                                Dosis = receta.Dosis,
                                Frecuencia = receta.Frecuencia,
                                Indicaciones = receta.Indicaciones,

                                Codigo = vademecum.Codigo,
                                CodigoTroquel = vademecum.CodigoTroquel,
                                PrincipioActivo = vademecum.PrincipioActivo,
                                NombreComercial = vademecum.NombreComercial,
                                Potencia = vademecum.Potencia,
                                Contenido = vademecum.Contenido,
                                FormaFarmaceutica = vademecum.FormaFarmaceutica,
                                Laboratorio = vademecum.Laboratorio,
                                Observaciones = vademecum.Observaciones,

                                NoSustituir = receta.NoSustituir
                            }, tran);

                            recetasAImprimir.Add((int)idEvolucionReceta);
                        }
                    }
                }

                //Órdenes
                if (model.Ordenes != null && model.Ordenes.Count > 0)
                {
                    foreach (var orden in model.Ordenes)
                    {
                        var idEvolucionOrden = await _evolucionesOrdenesRepository.Insert(new EvolucionOrden
                        {
                            IdEvolucion = (int)id,
                            Fecha = DateTime.Now,
                            Descripcion = orden.Descripcion,
                            Indicaciones = orden.Indicaciones
                        }, tran);

                        ordenesAImprimir.Add((int)idEvolucionOrden);
                    }
                }

                //Imagenes
                if (model.Imagenes != null && model.Imagenes.Count > 0)
                {
                    foreach (var imagen in model.Imagenes)
                    {
                        using (var ms = new MemoryStream())
                        {
                            imagen.CopyTo(ms);
                            var fileBytes = ms.ToArray();

                            var fileSignature = new FileSignature(fileBytes);
                            var file = fileSignature.Parse();

                            if (file != null)
                            {
                                if (!_allowedImageTypes.Contains(file.MimeType.ToLowerInvariant()))
                                {
                                    throw new BusinessException("Tipo de imagen inválida.");
                                }

                                var idArchivo = await _archivosRepository.Insert(new Archivo
                                {
                                    Fecha = DateTime.Now,
                                    Nombre = imagen.FileName,
                                    Tipo = file.MimeType,
                                    Extension = file.Extension,
                                    Contenido = fileBytes
                                }, tran);

                                await _evolucionesArchivosRepository.Insert(new EvolucionArchivo
                                {
                                    IdArchivo = (int)idArchivo,
                                    IdEvolucion = (int)id
                                }, tran);
                            }
                            else
                            {
                                throw new BusinessException("Una o mas imágenes inválidas.");
                            }
                        }
                    }
                }

                //Actualizo presupuestos
                if (!string.IsNullOrEmpty(model.Presupuestos))
                {
                    var lPresupuestos = model.Presupuestos.Split(",");
                    if (lPresupuestos.Length > 0)
                    {
                        foreach (var lP in lPresupuestos)
                        {
                            var presupuesto = await _presupuestosRepository.GetById<Presupuesto>(int.Parse(lP), tran);
                            presupuesto.IdEvolucion = (int)id;
                            await _presupuestosRepository.Update(presupuesto, tran);
                        }
                    }
                }

                //Actualizo el turno
                //turno.IdEstadoTurno = (int)EstadoTurno.Atendido;
                //await _turnosRepository.Update(turno, tran);

                //Agrego historial
                //await _turnosHistorialRepository.Insert(new TurnoHistorial
                //{
                //    Fecha = DateTime.Now,
                //    IdEstadoTurno = (int)EstadoTurno.Atendido,
                //    IdTurno = turno.IdTurno,
                //    IdUsuario = _permissionsBusiness.Value.User.Id,
                //    Observaciones = string.Empty
                //}, tran);

                if (commitTranHere)
                    tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new BusinessException(ex.Message);
            }

            if (recetasAImprimir.Count > 0 || ordenesAImprimir.Count > 0)
            {
                var recetas = new Dictionary<string, string>();
                foreach (var idEvolucionReceta in recetasAImprimir)
                {
                    var pdf = await _impresionesBusiness.GenerarImpresionReceta(idEvolucionReceta, idEvolucion);
                    var sPdf = Convert.ToBase64String(pdf.Content);

                    recetas.Add($"{idEvolucionReceta}.pdf", sPdf);
                }

                foreach (var idEvolucionOrden in ordenesAImprimir)
                {
                    var pdf = await _impresionesBusiness.GenerarImpresionOrden(idEvolucionOrden, idEvolucion);
                    var sPdf = Convert.ToBase64String(pdf.Content);

                    recetas.Add($"{idEvolucionOrden}.pdf", sPdf);
                }

                //var pdfIndicaciones = await _impresionesBusiness.GenerarImpresionReceta(0, idEvolucion);
                //var sPdfIndicaciones = Convert.ToBase64String(pdfIndicaciones.Content);

                //recetas.Add(pdfIndicaciones.Filename, sPdfIndicaciones);

                _mailBusiness.EnviarMailRecetasPaciente(paciente.Email, paciente.Nombre, recetas);
            }
        }

        public async Task<EvolucionesImportarExcelResultViewModel> ImportarEvolucionesExcel(IFormFile file)
        {
            var iResults = new EvolucionesImportarExcelResultViewModel();
            var memoryStream = new MemoryStream();

            try
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // <-- Add this, to make it work
                var eWrapper = new ExcelWrapper(memoryStream);
                var excelRows = eWrapper.GetListFromSheet<EvolucionesXLSViewModel>();

                //if (excelRows.Count > 50)
                //    throw new BusinessException("Solo se puede Importar la cantidad de 50 Registros. Utilice mas de un Archivo XLS para Importar la información de Saldo Inicio");

                var excelRowsGroupped = excelRows.GroupBy(x => new { x.Fecha, x.Documento });

                var resultados = new List<EvolucionExternaViewModel>();
                foreach (var group in excelRowsGroupped)
                {
                    if (string.IsNullOrEmpty(group.FirstOrDefault().Documento))
                        throw new BusinessException("Documento inválido.");

                    if (string.IsNullOrEmpty(group.FirstOrDefault().Diagnostico))
                        throw new BusinessException("Diagnóstico inválido.");

                    if (group.FirstOrDefault().Fecha == null)
                        throw new BusinessException("Fecha inválida.");

                    var paciente = await _pacientesRepository.GetPacienteByDocumento(group.FirstOrDefault().Documento);
                    if (paciente == null)
                    {
                        if (string.IsNullOrEmpty(group.FirstOrDefault().Nombre)
                        || string.IsNullOrEmpty(group.FirstOrDefault().Telefono)
                        || string.IsNullOrEmpty(group.FirstOrDefault().Email)
                            || group.FirstOrDefault().FechaNacimiento == null)
                        {
                            throw new BusinessException("Datos de paciente inválidos.");
                        }
                    }

                    FinanciadorPlan plan = null;
                    if (!string.IsNullOrEmpty(group.FirstOrDefault().Plan))
                    { 
                        plan = await _financiadoresPlanesRepository.GetByCodigo(group.FirstOrDefault().Plan);

                        if (plan == null)
                            throw new BusinessException(string.Format("Plan inválido {0}", group.FirstOrDefault().Plan));
                    }

                    var prestaciones = new List<EvolucionPrestacionViewModel>();
                    var otrasPrestaciones = new List<EvolucionOtraPrestacionViewModel>();

                    foreach (var item in group)
                    { 
                        if (!string.IsNullOrEmpty(item.Prestacion))
                        {
                            var prestacion = await _financiadorPrestacionesRepository.GetByCodigo(item.Prestacion);
                            if (prestacion == null)
                                throw new BusinessException(string.Format("No se encontró la prestacion {0}, para el financiador.", item.Prestacion));

                            prestaciones.Add(new EvolucionPrestacionViewModel
                            {
                                IdPrestacion = prestacion.IdFinanciadorPrestacion,
                                Pieza = item.Pieza
                            });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.PrestacionParticular))
                            {
                                var otraPrestacion = await _prestacionesRepository.GetByCodigo(item.PrestacionParticular);
                                if (otraPrestacion == null)
                                    throw new BusinessException(string.Format("No se encontró la prestacion {0} particular.", item.PrestacionParticular));

                                otrasPrestaciones.Add(new EvolucionOtraPrestacionViewModel
                                {
                                    IdOtraPrestacion = otraPrestacion.IdPrestacion,
                                    Pieza = item.Pieza
                                });
                            }
                            else
                            {                                
                                throw new BusinessException(string.Format("No se ingresaron prestaciones para el paciente {0}, fecha {1}.", item.Documento, item.Fecha));
                            }                                
                        }

                        item.Afiliado = "";
                    }

                    resultados.Add(new EvolucionExternaViewModel
                    {
                        Fecha = group.FirstOrDefault().Fecha,
                        Diagnostico = group.FirstOrDefault().Diagnostico,

                        IdPaciente = paciente != null ? (Int32)paciente.IdPaciente : 0,
                        Documento = paciente == null ? group.FirstOrDefault().Documento : string.Empty,
                        Nombre = paciente == null ? group.FirstOrDefault().Nombre : string.Empty,
                        Telefono = paciente == null ? group.FirstOrDefault().Telefono : string.Empty,
                        Email = paciente == null ? group.FirstOrDefault().Email : string.Empty,
                        FechaNacimiento = paciente == null ? group.FirstOrDefault().FechaNacimiento : null,

                        IdFinanciador = plan?.IdFinanciador,
                        IdFinanciadorPlan = plan?.IdFinanciadorPlan,
                        FinanciadorNro = plan == null ? null : group.FirstOrDefault().Afiliado,

                        Prestaciones = prestaciones,
                        OtrasPrestaciones = otrasPrestaciones,

                        IdProfesional = _permissionsBusiness.Value.User.IdProfesional
                    }); ;
                }                

                if (resultados == null || resultados.Count == 0)
                    throw new BusinessException($"No se encontraron registros en el archivo seleccionado.");

                var tran = _uow.BeginTransaction();

                var fechaPadron = DateTime.Now;
                foreach (var resultado in resultados)
                {
                    iResults.Cantidad++;

                    await NewExterna(resultado, tran);

                    iResults.Procesados++;
                }
                tran.Commit();

                return iResults;
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, null);
                throw ex;
            }
            catch (InvalidCastException ex)
            {
                _logger.LogError(ex, null);
                throw new BusinessException(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Remove(int idEvolucion)
        {
            var evolucion = await _evolucionesRepository.GetById<Evolucion>(idEvolucion);

            if (evolucion == null)
            {
                throw new ArgumentException("Evolucion inexistente");
            }

            if (evolucion.IdTurno.HasValue)
            {
                throw new BusinessException("No se puede eliminar la evolución");
            }

            //TODO:
            //Validar que no haya sido liquidada aún...

            var tran = _uow.BeginTransaction();

            try
            {
                evolucion.IdEstadoEvolucion = (int)EstadoEvolucion.Eliminada;
                evolucion.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                await _evolucionesRepository.Update(evolucion, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public Task Update(EvolucionViewModel model)
        {
            throw new NotImplementedException();
        }

        private void Validate(Evolucion evolucion)
        {
            if (evolucion.Diagnostico.IsNull())
            {
                throw new BusinessException("Ingrese un diagnóstico válido para la evolución.");
            }

            //if (profesional.Matricula.IsNull() && profesional.MatriculaProvincial.IsNull())
            //{
            //    throw new BusinessException("Ingrese una matrícula para el Profesional");
            //}

            //if (profesional.CUIT.IsNull() && profesional.IdTipoIVA != (int)TipoIVA.ConsumidorFinal)
            //{
            //    throw new BusinessException("Ingrese un CUIT Válido");
            //}

            //if (profesional.Telefono.IsNull())
            //{
            //    throw new BusinessException("Ingrese un Teléfono para el Profesional");
            //}

            //if (profesional.Email.IsNull())
            //{
            //    throw new BusinessException("Ingrese un E-Mail para el Profesional");
            //}

            //if (profesional.Direccion.IsNull())
            //{
            //    throw new BusinessException("Ingrese un Dirección para el Profesional");
            //}
        }

        public async Task<List<ResumenPrestacionesViewModel>> GetResumenPrestaciones(int idEvolucion)
        {
            //todo: validar que sea el mismo paciente por ej...

            var prestaciones = await _evolucionesPrestacionesRepository.GetAllByIdEvolucion(idEvolucion);
            var otrasPrestaciones = await _evolucionesOtrasPrestacionesRepository.GetAllByIdEvolucion(idEvolucion);

            var prestacionesModel = new List<ResumenPrestacionesViewModel>();

            if (prestaciones != null && prestaciones.Count > 0)
            {
                foreach(var prestacion in prestaciones)
                {
                    if (prestacion.IdTipoPrestacion != (int)TipoPrestacion.CoPago)
                    { 
                        prestacionesModel.Add(new ResumenPrestacionesViewModel { Pieza = prestacion.Pieza, Codigo = prestacion.Codigo, Descripcion = prestacion.Descripcion });
                    }
                }
            }

            if (otrasPrestaciones != null && otrasPrestaciones.Count > 0)
            {
                foreach (var prestacion in otrasPrestaciones)
                {
                    prestacionesModel.Add(new ResumenPrestacionesViewModel {Pieza = prestacion.Pieza, Codigo = prestacion.Codigo, Descripcion = prestacion.Descripcion });
                }
            }

            return prestacionesModel;
        }

        public async Task<List<ResumenImagenesViewModel>> GetResumenImagenes(int idEvolucion)
        {
            //todo: validar que sea el mismo paciente por ej...
            var imagenesModel = new List<ResumenImagenesViewModel>();

            var eArchivos = await _evolucionesArchivosRepository.GetAllByIdEvolucion(idEvolucion);
            if (eArchivos != null && eArchivos.Count > 0)
            {
                foreach (var eArchivo in eArchivos)
                {
                    var archivo = await _archivosRepository.GetById<Archivo>(eArchivo.IdArchivo);

                    var imagen = Convert.ToBase64String(archivo.Contenido);
                    imagenesModel.Add(new ResumenImagenesViewModel { Nombre = archivo.Nombre, 
                                                                        Tipo = archivo.Tipo, 
                                                                        Extension = archivo.Extension,
                                                                        Imagen = imagen
                                                                    });
                }
            }

            return imagenesModel;
        }

        public async Task<List<ResumenRecetasViewModel>> GetResumenRecetas(int idEvolucion)
        {
            //todo: validar que sea el mismo paciente por ej...

            var recetas = await _evolucionesRecetasRepository.GetAllByIdEvolucion(idEvolucion);

            var recetasModel = new List<ResumenRecetasViewModel>();

            if (recetas != null && recetas.Count > 0)
            {
                foreach (var receta in recetas)
                {
                    recetasModel.Add(new ResumenRecetasViewModel { IdEvolucionReceta = receta.IdEvolucionReceta, Codigo = receta.Codigo, Descripcion = receta.Descripcion });
                }
                //recetasModel.Add(new ResumenRecetasViewModel { IdEvolucionReceta = 0, Codigo = 0, Descripcion = "INDICACIONES" });
            }
            return recetasModel;
        }

        public async Task<List<ResumenOrdenesViewModel>> GetResumenOrdenes(int idEvolucion)
        {
            //todo: validar que sea el mismo paciente por ej...

            var ordenes = await _evolucionesOrdenesRepository.GetAllByIdEvolucion(idEvolucion);

            var ordenesModel = new List<ResumenOrdenesViewModel>();

            if (ordenes != null && ordenes.Count > 0)
            {
                foreach (var orden in ordenes)
                {
                    ordenesModel.Add(new ResumenOrdenesViewModel { IdEvolucionOrden = orden.IdEvolucionOrden, Descripcion = orden.Descripcion });
                }
            }
            return ordenesModel;
        }
        public async Task<List<Custom.Evolucion>> GetEvolucionesPaciente()
        => await _evolucionesRepository.GetEvolucionesPaciente(_permissionsBusiness.Value.User.IdPaciente);

		public async Task<OdontogramaViewModel> GetOdontograma(int idEvolucion)
		{
			//todo: validar que sea el mismo paciente por ej...

			var piezas = await _evolucionesOdontogramasPiezasRepository.GetAllByIdEvolucion(idEvolucion); 
            var zonas = await _evolucionesOdontogramasPiezasZonasRepository.GetAllByIdEvolucion(idEvolucion);

			OdontogramaViewModel odontogramaModel = null;

            if (piezas != null && piezas.Count > 0)
            {
                odontogramaModel = new OdontogramaViewModel
                {
                    IdEvolucion = idEvolucion,
                    Piezas = new List<OdontogramaPiezaViewModel>()
                };

                foreach (var pieza in piezas)
                {
                    var pZonas = new List<OdontogramaZonaViewModel>();

                    var piezaZonas = zonas.Where(z => z.Pieza == pieza.Pieza);
                    foreach (var zona in piezaZonas)
                    {
                        pZonas.Add(new OdontogramaZonaViewModel
                        {
                            Pieza = zona.Pieza,
                            Zona = zona.Zona,
                            TipoTrabajo = zona.IdTipoTrabajoOdontograma
                        });
                    }

                    odontogramaModel.Piezas.Add(new OdontogramaPiezaViewModel
                    {
                        Zonas = pZonas,
                        Pieza = pieza.Pieza,
                        Caries = pieza.Caries,
                        Corona = pieza.Corona,
                        PrFija = pieza.PrFija,
                        PrRemovible = pieza.PrRemovible,
                        Amalgama = pieza.Amalgama,
                        Ausente = pieza.Ausente,
                        Extraccion = pieza.Extraccion,
                        Ortodoncia = pieza.Ortodoncia,
                        Observaciones = pieza.Observaciones
                    });
                }
            }
            else
                odontogramaModel = new OdontogramaViewModel { IdEvolucion = idEvolucion };

			return odontogramaModel;
		}

		public async Task<OdontogramaViewModel> GetUltimoOdontograma(int idPaciente)
		{
            //todo: validar que sea el mismo paciente por ej...
            var evolucionesPaciente = await _evolucionesRepository.GetEvolucionesPaciente(idPaciente);
            var idUltimaEvolucionPaciente = 0;
            if (evolucionesPaciente!= null && evolucionesPaciente.Count > 0)
				idUltimaEvolucionPaciente = evolucionesPaciente.Max(e => e.IdEvolucion);

            var odontograma = await GetOdontograma(idUltimaEvolucionPaciente);
            if (odontograma != null)
            {
                odontograma.IdEvolucion = 0;

                foreach (var pieza in odontograma.Piezas)
                {
                    foreach (var zona in pieza.Zonas)
                    {
                        if (zona.TipoTrabajo > 0)
                            zona.TipoTrabajo = 2;
                    }
                }
            }

			return odontograma;
		}

        public async Task<List<Custom.PrestacionProfesional>> GetPrestacionesProfesional(DataTablesRequest request)
        {
            var filters = request.ParseFilters();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            return await _evolucionesOtrasPrestacionesRepository.GetPrestacionesProfesional(filters);
        }

        public async Task<List<Custom.PrestacionFinanciador>> GetPrestacionesFinanciador(DataTablesRequest request)
        {
            var filters = request.ParseFilters();
            filters.Add("IdEmpresa", _permissionsBusiness.Value.User.IdEmpresa);

            return await _evolucionesPrestacionesRepository.GetPrestacionesFinanciador(filters);
        }

        public async Task<Custom.EvolucionesEstadisticas> GetEstadisticas(EvolucionesEstadisticasViewModel filters)
        {
            return await _evolucionesRepository.GetEstadisticas(_permissionsBusiness.Value.User.IdEmpresa,
                                                                DateTime.ParseExact(filters.FechaDesde, "dd/MM/yyyy", null),
                                                                DateTime.ParseExact(filters.FechaHasta, "dd/MM/yyyy", null),
                                                                filters.IdProfesional, filters.IdEspecialidad);
        }

        public async Task<List<Custom.EvolucionesEstadisticasFinanciador>> GetEstadisticasFinanciador(EvolucionesEstadisticasViewModel filters)
        {
            return await _evolucionesRepository.GetEstadisticasFinanciador(_permissionsBusiness.Value.User.IdEmpresa,
                                                                            DateTime.ParseExact(filters.FechaDesde, "dd/MM/yyyy", null),
                                                                            DateTime.ParseExact(filters.FechaHasta, "dd/MM/yyyy", null),
                                                                            filters.IdProfesional, filters.IdEspecialidad);
        }

        public async Task<List<Custom.EvolucionesEstadisticasEspecialidad>> GetEstadisticasEspecialidad(EvolucionesEstadisticasViewModel filters)
        {
            return await _evolucionesRepository.GetEstadisticasEspecialidad(_permissionsBusiness.Value.User.IdEmpresa, 
                                                                            DateTime.ParseExact(filters.FechaDesde, "dd/MM/yyyy", null),
                                                                            DateTime.ParseExact(filters.FechaHasta, "dd/MM/yyyy", null),
                                                                            filters.IdProfesional, filters.IdEspecialidad);
        }

        private async Task<int> NewPacienteCliente(Paciente paciente, IDbTransaction transaction = null)
        {
            var idPaciente = await _pacientesRepository.Insert(paciente, transaction);

            var cliente = new Cliente
            {
                IdPaciente = (int)idPaciente,
                RazonSocial = paciente.Nombre,
                NombreFantasia = paciente.Nombre,
                IdTipoDocumento = (int)TipoDocumento.DNI,
                Documento = paciente.Documento,
                IdTipoIVA = (int)TipoIVA.ConsumidorFinal,
                Email = paciente.Email,
                IdTipoTelefono = (int)TipoTelefono.Movil,
                Telefono = paciente.Telefono,
                IdEstadoRegistro = (int)EstadoRegistro.Nuevo
            };

            var idCliente = await _clientesRepository.Insert(cliente, transaction);

            paciente.IdPaciente = (int)idPaciente;
            await NewUsuario(paciente, transaction);

            //_chatApiBusiness.SendMessageBienvenida(paciente.Telefono, paciente.Nombre);
            //_mailBusiness.EnviarMailBienvenidaPaciente(paciente.Email, paciente.Nombre);

            return (int)idPaciente;
        }

        private async Task NewUsuario(Paciente paciente, IDbTransaction transaction = null)
        {
            var salt = Cryptography.GenerateSalt();
            var password = Cryptography.GetTempPassword();
            var usuario = new Usuario
            {
                Login = paciente.Documento,
                Nombre = paciente.Nombre,
                Email = paciente.Email,
                Password = Cryptography.HashPassword(password, salt),
                PasswordSalt = Convert.ToBase64String(salt),
                BlankToken = null,
                Vencimiento = DateTime.Now.AddDays(-1),
                IdTipoUsuario = (int)TipoUsuario.Paciente,
                IdPaciente = paciente.IdPaciente,
                IdEstadoRegistro = (int)EstadoRegistro.Nuevo
            };

            await _usuariosRepository.Insert(usuario, transaction);
        }
    }
}
