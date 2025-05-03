using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.FullCalendar;
using LizardCode.SalmaSalud.Application.Models.Profesionales;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class ProfesionalesBusiness : BaseBusiness, IProfesionalesBusiness
    {
        private List<string> _allowedImageTypes = new List<string> { "image/jpeg", "image/gif", "image/png" };

        private readonly ILogger<ProfesionalesBusiness> _logger;
        private readonly IProfesionalesRepository _ProfesionalesRepository;
        private readonly IProfesionalesEmpresasRepository _ProfesionalesEmpresasRepository;
        private readonly IProfesionalesTurnosRepository _profesionalesTurnosRepository;
        private readonly IAfipAuthRepository _afipAuthRepository;
        private readonly IEmpresasRepository _empresasRepository;
        private readonly IEmpresasCertificadosRepository _empresasCertificadosRepository;
        private readonly IFeriadosRepository _feriadosRepository;
        private readonly IArchivosRepository _archivosRepository;
        private readonly IProveedoresRepository _proveedoresRepository;

        public ProfesionalesBusiness(
            IProfesionalesRepository profesionalesRepository,
            ILogger<ProfesionalesBusiness> logger,
            IAfipAuthRepository afipAuthRepository,
            IEmpresasRepository empresasRepository,
            IEmpresasCertificadosRepository empresasCertificadosRepository,
            IProfesionalesEmpresasRepository profesionalesEmpresasRepository,
            IProfesionalesTurnosRepository profesionalesTurnosRepository,
            IFeriadosRepository feriadosRepository,
            IArchivosRepository archivosRepository,
            IProveedoresRepository proveedoresRepository)
        {
            _ProfesionalesRepository = profesionalesRepository;
            _logger = logger;
            _ProfesionalesEmpresasRepository = profesionalesEmpresasRepository;
            _empresasRepository = empresasRepository;
            _empresasCertificadosRepository = empresasCertificadosRepository;
            _afipAuthRepository = afipAuthRepository;
            _profesionalesTurnosRepository = profesionalesTurnosRepository;
            _feriadosRepository = feriadosRepository;
            _archivosRepository = archivosRepository;
            _proveedoresRepository = proveedoresRepository;
        }


        public async Task New(ProfesionalViewModel model)
        {
            var profesional = _mapper.Map<Profesional>(model);
            var empresas = model.Empresas;

            Validate(profesional);

            var tran = _uow.BeginTransaction();

            try
            {
                //Busco si ya no Existe el Profesional en otra Empresa
                var dbProfesional = await _ProfesionalesRepository.GetProfesionalByCUIT(profesional.CUIT.ToUpper().Trim(), tran);

                if (dbProfesional == default)
                {
                    if (model.ExentoIIBB)
                    {
                        profesional.NroIBr = string.Empty;
                    }
                    else
                    {
                        profesional.NroIBr = profesional.NroIBr?.ToUpper().Trim() ?? string.Empty;
                    }

                    profesional.Nombre = profesional.Nombre.ToUpper().Trim();
                    profesional.IdTipoIVA = profesional.IdTipoIVA;
                    profesional.CUIT = profesional.CUIT.ToUpper().Trim();
                    profesional.Email = profesional.Email.ToLower().Trim();
                    profesional.Direccion = profesional.Direccion.ToUpper().Trim();
                    profesional.CodigoPostal = profesional.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                    profesional.Piso = profesional.Piso?.ToUpper().Trim() ?? string.Empty;
                    profesional.Departamento = profesional.Departamento?.ToUpper().Trim() ?? string.Empty;
                    profesional.Localidad = profesional.Localidad?.ToUpper().Trim() ?? string.Empty;
                    profesional.Provincia = profesional.Provincia?.ToUpper().Trim() ?? string.Empty;
                    profesional.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

                    profesional.PolizaNumero = model.PolizaNumero;
                    profesional.PolizaAseguradora = model.PolizaAseguradora;
                    profesional.PolizaVencimiento = model.PolizaVencimiento;

                    //FIRMA
                    if (model.Firma != null)
                    {
                        profesional.IdArchivoFirma = await SubirFirma(model.Firma, tran);
                    }

                    var id = await _ProfesionalesRepository.Insert(profesional, tran);

                    if (empresas != null && empresas.Count > 0)
                    {
                        foreach (var empresa in empresas)
                        {
                            await _ProfesionalesEmpresasRepository.Insert(new ProfesionalEmpresa() { IdProfesional = (int)id, IdEmpresa = empresa.Value }, tran);
                        }
                    }

                    profesional.IdProfesional = (int)id;
                    await NewProveedor(profesional, tran);
                }
                else
                {
                    if (model.ExentoIIBB)
                    {
                        dbProfesional.NroIBr = string.Empty;
                    }
                    else
                    {
                        dbProfesional.NroIBr = profesional.NroIBr?.ToUpper().Trim() ?? string.Empty;
                    }

                    dbProfesional.Nombre = profesional.Nombre.ToUpper().Trim();
                    dbProfesional.IdTipoIVA = profesional.IdTipoIVA;
                    dbProfesional.CUIT = profesional.CUIT.ToUpper().Trim();
                    //dbProfesional.NroIBr = profesional.NroIBr?.ToUpper().Trim() ?? string.Empty;
                    dbProfesional.Email = profesional.Email.ToLower().Trim();
                    dbProfesional.Direccion = profesional.Direccion.ToUpper().Trim();
                    dbProfesional.CodigoPostal = profesional.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                    dbProfesional.Piso = profesional.Piso?.ToUpper().Trim() ?? string.Empty;
                    dbProfesional.Departamento = profesional.Departamento?.ToUpper().Trim() ?? string.Empty;
                    dbProfesional.Localidad = profesional.Localidad?.ToUpper().Trim() ?? string.Empty;
                    dbProfesional.Provincia = profesional.Provincia?.ToUpper().Trim() ?? string.Empty;
                    dbProfesional.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                    dbProfesional.PolizaNumero = profesional.PolizaNumero;
                    dbProfesional.PolizaAseguradora = profesional.PolizaAseguradora;
                    dbProfesional.PolizaVencimiento = profesional.PolizaVencimiento;

                    dbProfesional.CantidadSobreturnos = profesional.CantidadSobreturnos;
                    dbProfesional.TurnosIntervalo = profesional.TurnosIntervalo;

                    //FIRMA
                    if (model.Firma != null)
                    {
                        if (dbProfesional.IdArchivoFirma.HasValue)
                        {
                            await _archivosRepository.DeleteById<Archivo>(dbProfesional.IdArchivoFirma.Value, tran);
                            dbProfesional.IdArchivoFirma = null;
                        }

                        dbProfesional.IdArchivoFirma = await SubirFirma(model.Firma, tran);
                    }

                    await _ProfesionalesRepository.Update(dbProfesional, tran);

                    if (empresas != null && empresas.Count > 0)
                    {
                        foreach (var empresa in empresas)
                        {
                            await _ProfesionalesEmpresasRepository.Insert(new ProfesionalEmpresa() { IdProfesional = dbProfesional.IdProfesional, IdEmpresa = empresa.Value }, tran);
                        }
                    }

                    await UpdateProveedor(dbProfesional, tran);
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task<ProfesionalViewModel> Get(int idProfesional)
        {
            var profesional = await _ProfesionalesRepository.GetById<Profesional>(idProfesional);
            var empresas = await _ProfesionalesEmpresasRepository.GetAllByIdProfesional(idProfesional);

            if (profesional == null)
                return null;

            var model = _mapper.Map<ProfesionalViewModel>(profesional);
            model.Empresas = empresas.Select(e => e?.IdEmpresa).ToList();

            if (profesional.IdArchivoFirma.HasValue)
            {
                var archivo = await _archivosRepository.GetById<Archivo>(profesional.IdArchivoFirma.Value);
                if (archivo != null)
                {
                    model.UploadedFirma = Convert.ToBase64String(archivo.Contenido);
                    model.UploadedTipoFirma = archivo.Tipo;
                }
            }

            return model;
        }

        public async Task<DataTablesResponse<Custom.Profesional>> GetAll(DataTablesRequest request)
        {
            var customQuery = _ProfesionalesRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();
            var filters = request.ParseFilters();

            if (filters.ContainsKey("FiltroNombre"))
                builder.Append($"AND Nombre LIKE {string.Concat("%", filters["FiltroNombre"], "%")}");

            builder.Append($"AND idProfesional IN (SELECT idProfesional FROM ProfesionalesEmpresas WHERE IdEmpresa IN (SELECT IdEmpresa FROM UsuariosEmpresas WHERE IdUsuario = {_permissionsBusiness.Value.User.Id}))");

            foreach (var item in builder.Parameters)
                customQuery.Parameters.Add(item.Key, item.Value.Value);

            return await _dataTablesService.Resolve<Custom.Profesional>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task Update(ProfesionalViewModel model)
        {
            var profesional = _mapper.Map<Profesional>(model);
            var empresas = model.Empresas;

            Validate(profesional);

            var dbProfesional = await _ProfesionalesRepository.GetById<Profesional>(profesional.IdProfesional);

            if (dbProfesional == null)
            {
                throw new ArgumentException("Profesional inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                if (model.ExentoIIBB)
                {
                    dbProfesional.NroIBr = string.Empty;
                }
                else
                {
                    dbProfesional.NroIBr = profesional.NroIBr?.ToUpper().Trim() ?? string.Empty;
                }

                dbProfesional.Nombre = profesional.Nombre.ToUpper().Trim();
                dbProfesional.CUIT = profesional.CUIT.ToUpper().Trim();
                dbProfesional.IdTipoIVA = profesional.IdTipoIVA;
                dbProfesional.Email = profesional.Email.ToLower().Trim();
                dbProfesional.Direccion = profesional.Direccion.ToUpper().Trim();
                dbProfesional.CodigoPostal = profesional.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
                dbProfesional.Piso = profesional.Piso?.ToUpper().Trim() ?? string.Empty;
                dbProfesional.Departamento = profesional.Departamento?.ToUpper().Trim() ?? string.Empty;
                dbProfesional.Localidad = profesional.Localidad?.ToUpper().Trim() ?? string.Empty;
                dbProfesional.Provincia = profesional.Provincia?.ToUpper().Trim() ?? string.Empty;
                dbProfesional.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

                dbProfesional.PolizaNumero = profesional.PolizaNumero;
                dbProfesional.PolizaAseguradora = profesional.PolizaAseguradora;
                dbProfesional.PolizaVencimiento = profesional.PolizaVencimiento;

                dbProfesional.CantidadSobreturnos = profesional.CantidadSobreturnos; 
                dbProfesional.TurnosIntervalo = profesional.TurnosIntervalo;

                //FIRMA
                if (model.RemovedFirma && dbProfesional.IdArchivoFirma.HasValue)
                {
                    await _archivosRepository.DeleteById<Archivo>(dbProfesional.IdArchivoFirma.Value, tran);
                    dbProfesional.IdArchivoFirma = null;
                }

                if (model.Firma != null)
                {
                    dbProfesional.IdArchivoFirma = await SubirFirma(model.Firma, tran);
                }

                await _ProfesionalesRepository.Update(dbProfesional, tran);

                await _ProfesionalesEmpresasRepository.RemoveByIdProfesional(dbProfesional.IdProfesional, tran);
                if (empresas != null && empresas.Count > 0)
                {
                    foreach (var empresa in empresas)
                    {
                        await _ProfesionalesEmpresasRepository.Insert(new ProfesionalEmpresa() { IdProfesional = (int)dbProfesional.IdProfesional, IdEmpresa = empresa.Value }, tran);
                    }
                }

                await UpdateProveedor(dbProfesional, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task Remove(int idProfesional)
        {
            var profesional = await _ProfesionalesRepository.GetById<Profesional>(idProfesional);

            if (profesional == null)
            {
                throw new ArgumentException("Profesional inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                await _ProfesionalesEmpresasRepository.RemoveByIdProfesional(profesional.IdProfesional, tran);

                //Verifico que el Profesional no tenga Relacion en otra empresa.
                var lstProfesionales = await _ProfesionalesEmpresasRepository.GetAllByIdProfesional(profesional.IdProfesional, tran);
                if (lstProfesionales.Count == 0)
                {
                    profesional.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;
                    await _ProfesionalesRepository.Update(profesional, tran);
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        private void Validate(Profesional profesional)
        {
            if (profesional.Nombre.IsNull())
            {
                throw new BusinessException("Ingrese el Nombre de Fantasia para Profesional");
            }

            if (profesional.Matricula.IsNull() && profesional.MatriculaProvincial.IsNull())
            {
                throw new BusinessException("Ingrese una matrícula para el Profesional");
            }

            if (profesional.CUIT.IsNull() && profesional.IdTipoIVA != (int)TipoIVA.ConsumidorFinal)
            {
                throw new BusinessException("Ingrese un CUIT Válido");
            }

            if (profesional.Telefono.IsNull())
            {
                throw new BusinessException("Ingrese un Teléfono para el Profesional");
            }

            if (profesional.Email.IsNull())
            {
                throw new BusinessException("Ingrese un E-Mail para el Profesional");
            }

            if (profesional.Direccion.IsNull())
            {
                throw new BusinessException("Ingrese un Dirección para el Profesional");
            }
        }

        public async Task<string> ValidarNroCUIT(string cuit, int? idProfesional)
        {
            if (!cuit.ValidarCUIT())
            {
                return new string("C.U.I.T. Invalido");
            }

            var result = await _ProfesionalesRepository.ValidarCUITExistente(cuit, idProfesional, _permissionsBusiness.Value.User.IdEmpresa);
            if (result)
                return new string("Existe un Profesional con el nro de C.U.I.T. Ingresado. Verifique");
            return null;
        }

        public async Task<Custom.Contribuyente> GetPadron(string cuit)
        {
            try
            {
                if (!cuit.ValidarCUIT())
                {
                    throw new BusinessException("C.U.I.T. Inválido");
                }

                var empresa = await _empresasRepository.GetById<Empresa>(_permissionsBusiness.Value.User.IdEmpresa);
                var afipAuth = await _afipAuthRepository.GetValidSignToken(_permissionsBusiness.Value.User.IdEmpresa, ServicioAFIP.WEB_SERVICE_PADRON.Description());
                var cuitEmpresa = string.Empty;
                var padronProd = true;

                if (afipAuth == null)
                {
                    var tran = _uow.BeginTransaction();

                    try
                    {
                        var crt = await _empresasCertificadosRepository.GetValidEmpresaCerificadoByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa, tran);

                        if (crt == default)
                        {
                            var genericCrt = "AFIP-CRT".FromAppSettings<string>(notFoundException: true);
                            var genericPk = "AFIP-PK".FromAppSettings<string>(notFoundException: true);
                            var genericCuit = "AFIP-CUIT".FromAppSettings<string>(notFoundException: true);

                            cuitEmpresa = genericCuit;
                            afipAuth = await _afipAuthRepository.NewSignToken(_permissionsBusiness.Value.User.IdEmpresa, genericCrt, genericPk, genericCuit, ServicioAFIP.WEB_SERVICE_PADRON.Description(), true);
                        }
                        else
                        {
                            cuitEmpresa = empresa.CUIT.Replace("-", string.Empty);
                            afipAuth = await _afipAuthRepository.NewSignToken(_permissionsBusiness.Value.User.IdEmpresa, crt.CRT, empresa.PrivateKey, cuitEmpresa, ServicioAFIP.WEB_SERVICE_PADRON.Description(), empresa.EnableProdAFIP);
                            padronProd = empresa.EnableProdAFIP;
                        }

                        await _afipAuthRepository.Insert(afipAuth, tran);
                        tran.Commit();

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, null);
                        tran.Rollback();
                        throw new BusinessException(ex.Message);
                    }
                }

                Custom.Contribuyente contribuyente;
                try
                {
                    contribuyente = await _empresasRepository.GetPadronByCUIT(afipAuth, afipAuth.CUIT, cuit.Replace("-", string.Empty), padronProd);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, null);
                    throw new BusinessException(ex.Message);
                }

                var respInsc = Enum.GetValues(typeof(ImpuestosAFIPRespInscripto)).Cast<ImpuestosAFIPRespInscripto>().ToList()
                    .Any(i => contribuyente.Impuestos.Contains((int)i));

                var monotributo = Enum.GetValues(typeof(ImpuestosAFIPMonotributo)).Cast<ImpuestosAFIPMonotributo>().ToList()
                    .Any(i => contribuyente.Impuestos.Contains((int)i));

                var exento = Enum.GetValues(typeof(ImpuestosAFIPExento)).Cast<ImpuestosAFIPExento>().ToList()
                    .Any(i => contribuyente.Impuestos.Contains((int)i));

                contribuyente.IdTipoIVA = 0;
                if (respInsc)
                    contribuyente.IdTipoIVA = (int)TipoIVA.ResponsableInscripto;
                if (monotributo)
                    contribuyente.IdTipoIVA = (int)TipoIVA.Monotributo;
                if (exento)
                    contribuyente.IdTipoIVA = (int)TipoIVA.Exento;

                return contribuyente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw new BusinessException(ex.Message);
            }
        }

        public async Task<List<ProfesionalTurnoEvent>> GetAgenda(DateTime desde, DateTime hasta, int idProfesional)
        {
            List<ProfesionalTurnoEvent> events = new List<ProfesionalTurnoEvent>();

            var profesional = await _ProfesionalesRepository.GetById<Profesional>(idProfesional);
            var especialidad = (await _lookupsBusiness.Value.GetAllEspecialidades()).FirstOrDefault(f=> f.IdEspecialidad == profesional.IdEspecialidad);

            var empresa = (await _lookupsBusiness.Value.GetAllEmpresasLookup()).FirstOrDefault(f => f.IdEmpresa == _permissionsBusiness.Value.User.IdEmpresa);
            var hDesde = empresa.TurnosHoraInicio.Split(":");
            var hHasta = empresa.TurnosHoraFin.Split(":");

            var iIntervaloTrunos = profesional.TurnosIntervalo > 0 ? profesional.TurnosIntervalo : (especialidad.TurnosIntervalo == 0 ? empresa.TurnosIntervalo : especialidad.TurnosIntervalo);
            var sIntervaloTrunos = iIntervaloTrunos.ToString();

            var horaDesde = int.Parse(hDesde[0]);
            var horaHasta = int.Parse(hHasta[0]);

            desde = desde.AddHours(int.Parse(hDesde[0])).AddMinutes(int.Parse(hDesde[1]));
            hasta = hasta.AddHours(int.Parse(hHasta[0])).AddMinutes(int.Parse(hHasta[1]));

            //Turnos UTILIZADOS
            var turnos = await _profesionalesTurnosRepository.GetAllByIdProfesionalAndIdEmpresa(desde < DateTime.Now ? DateTime.Now : desde, hasta, idProfesional, _permissionsBusiness.Value.User.IdEmpresa);
            if (turnos != null && turnos.Count > 0)
            {
                foreach (var turno in turnos)
                {
                    if (turno.IdEstadoProfesionalTurno == (int)EstadoProfesionalTurno.Agendado)
                    {
                        events.Add(new ProfesionalTurnoEvent
                        {
                            IdProfesionalTurno = -1,
                            Fecha = turno.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                            ClassNames = "fc-event-custom fc-event-warning",
                            Title = "T. AGENDADO",
                            Start = turno.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                            End = turno.FechaFin.ToString("yyyy-MM-ddTHH:mm:ss"),
                            AllDay = false
                        });
                    }
                    else
                    {
                        events.Add(new ProfesionalTurnoEvent
                        {
                            IdProfesionalTurno =  turno.IdProfesionalTurno,
                            Fecha = turno.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                            ClassNames = "fc-event-custom fc-event-odonto",
                            Title = "HABILITADO",
                            Start = turno.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                            End = turno.FechaFin.ToString("yyyy-MM-ddTHH:mm:ss"),
                            AllDay = false
                        });
                    }
                }
            }

            //Turnos DISPONIBLES{
            while (desde <= hasta)
            {
                if (desde >= DateTime.Now)
                { 
                    if (desde.Hour >= horaDesde && desde.Hour <= horaHasta) { 
                        //Validar además que este en el intervalo que trabaja la institución...
                        //Validar que no haya un utilizado...
                        var disponible = true;
                        if (turnos != null && turnos.Count > 0)
                        {
                            disponible = !turnos.Any(t => desde >= t.FechaInicio && desde.AddMinutes(iIntervaloTrunos) <= t.FechaFin);
                        }

                        if (disponible)
                        { 
                            events.Add(new ProfesionalTurnoEvent
                            {
                                Fecha = desde.ToString("yyyy-MM-ddTHH:mm:ss"),
                                IdProfesionalTurno = 0,
                                ClassNames = "fc-event-custom fc-event-gris-claro",
                                Title = "DISPONIBLE",
                                Start = desde.ToString("yyyy-MM-ddTHH:mm:ss"),
                                End = desde.AddMinutes(iIntervaloTrunos).ToString("yyyy-MM-ddTHH:mm:ss"),
                                AllDay = false
                            });
                        }
                    }
                }

                desde = desde.AddMinutes(iIntervaloTrunos);
            }

            //Feriados/Eventos
            //TODO: Hacer con rango de fechas... no "ALL"...
            var feriadosEventos = await _feriadosRepository.GetAllByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa);
            if (feriadosEventos != null && feriadosEventos.Count > 0)
            {
                foreach (var fe in feriadosEventos)
                {
                    events.Add(new ProfesionalTurnoEvent
                    {
                        Fecha = fe.Fecha.ToString("yyyy-MM-ddTHH:mm:ss"),
                        Clickable = false,
                        IdProfesionalTurno = -1,
                        ClassNames = "fc-event-custom fc-event-warning",
                        Title = fe.Nombre.ToUpperInvariant(),
                        Start = fe.Fecha.ToString("yyyy-MM-ddTHH:mm:ss"),
                        //End = fe.Fecha.ToString("yyyy-MM-ddTHH:mm:ss"),
                        Display = "background",
                        AllDay = true
                    });
                }

            }

            return events;
        }

        public async Task<int> HabilitarCargaAgenda(int idProfesional)
        {
            var profesional = await _ProfesionalesRepository.GetById<Profesional>(idProfesional);
            var empresas = await _ProfesionalesEmpresasRepository.GetAllByIdProfesional(idProfesional);

            if (profesional.TurnosIntervalo > 0)
            {
                return profesional.TurnosIntervalo;
            }
            else
            { 
                if (empresas != null && empresas.Count > 0 
                    && empresas.Any(e => e.IdEmpresa == _permissionsBusiness.Value.User.IdEmpresa))
                {
                    //var profesional = await _ProfesionalesRepository.GetById<Profesional>(idProfesional);
                    var empresa = await _empresasRepository.GetById<Empresa>(_permissionsBusiness.Value.User.IdEmpresa);
                    var especialidad = (await _lookupsBusiness.Value.GetAllEspecialidades()).FirstOrDefault(f => f.IdEspecialidad == profesional.IdEspecialidad);

                    return especialidad.TurnosIntervalo > 0 ? especialidad.TurnosIntervalo : empresa.TurnosIntervalo;
                }
                else
                    return 0;
            }
        }

        public async Task AddTurno(int idProfesional, DateTime fechaInicio)
        {
            await ValidateTurno(idProfesional, fechaInicio);

            var profesional = await _ProfesionalesRepository.GetById<Profesional>(idProfesional);
            var especialidad = (await _lookupsBusiness.Value.GetAllEspecialidades()).FirstOrDefault(f=> f.IdEspecialidad == profesional.IdEspecialidad);
            var empresa = (await _lookupsBusiness.Value.GetEmpresasByIdUsuarioLookup(_permissionsBusiness.Value.User.Id)).FirstOrDefault(f=> f.IdEmpresa == _permissionsBusiness.Value.User.IdEmpresa);

            var intervaloMinutos = profesional.TurnosIntervalo > 0 ? profesional.TurnosIntervalo : (especialidad.TurnosIntervalo > 0 ? especialidad.TurnosIntervalo : empresa.TurnosIntervalo);

            var tran = _uow.BeginTransaction();

            try
            {
                await _profesionalesTurnosRepository.Insert(new ProfesionalTurno() { IdProfesional = idProfesional, 
                                                                                        IdEspecialidad = profesional.IdEspecialidad,
                                                                                        IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa, 
                                                                                        FechaInicio = fechaInicio, 
                                                                                        FechaFin = fechaInicio.AddMinutes(intervaloMinutos),
                                                                                        IdEstadoProfesionalTurno = (int)EstadoProfesionalTurno.Disponible
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

        public async Task RemoveTurno(int idProfesionalTurno)
        {
            var pTurno = await _profesionalesTurnosRepository.GetById<ProfesionalTurno>(idProfesionalTurno);
            if (pTurno == null)
            {
                throw new ArgumentException("Turno inexistente");
            }

            if (pTurno.IdEstadoProfesionalTurno == (int)EstadoProfesionalTurno.Agendado)
            {
                throw new ArgumentException("Turno Agendado. No se puede eliminar.");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                await _profesionalesTurnosRepository.RemoveById(idProfesionalTurno, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task CopiarSemana(int idProfesional, DateTime fechaFin)
        {
            //await ValidateTurno(idProfesional, fechaInicio);
            //TODO: Arrancar todo desde las 00hs y demases...

            var tran = _uow.BeginTransaction();

            try
            {
                var fechaInicio = fechaFin.AddDays(-7);

                //Obtengo todos los habilitados de esta semana...
                var agendaHabilitada = await _profesionalesTurnosRepository.GetAllByIdProfesionalAndIdEmpresa(fechaInicio, fechaFin, idProfesional, _permissionsBusiness.Value.User.IdEmpresa, tran);
                if (agendaHabilitada != null && agendaHabilitada.Count > 0)
                {
                    var existenTurnosAgendados = await _profesionalesTurnosRepository.ExistenTurnosAgendadosByFecha(fechaFin, fechaFin.AddDays(7), idProfesional, _permissionsBusiness.Value.User.IdEmpresa, tran);
                    if (existenTurnosAgendados)
                        throw new BusinessException("Existen turnos agendados para el rango seleccionado.");

                    //Borro todo lo de la sig semana... 
                    await _profesionalesTurnosRepository.RemoveAllByFecha(fechaFin, fechaFin.AddDays(7), idProfesional, _permissionsBusiness.Value.User.IdEmpresa, tran);

                    //Agrego nuevos turnos
                    foreach (var aH in agendaHabilitada)
                    {
                        await _profesionalesTurnosRepository.Insert(new ProfesionalTurno()
                        {
                            IdProfesional = idProfesional,
                            IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                            FechaInicio = aH.FechaInicio.AddDays(7),
                            FechaFin = aH.FechaFin.AddDays(7),
                            IdEspecialidad = aH.IdEspecialidad,
                            IdEstadoProfesionalTurno = (int)EstadoProfesionalTurno.Disponible
                        }, tran);

                    }
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }
        
        public async Task CopiarDia(int idProfesional, DateTime dia)
        {
            //await ValidateTurno(idProfesional, fechaInicio);
            //TODO: Arrancar todo desde las 00hs y demases...

            var tran = _uow.BeginTransaction();

            try
            {
                var fechaInicio = dia.AddDays(-1);
                //Obtengo todos los habilitados de esta semana...
                var agendaHabilitada = await _profesionalesTurnosRepository.GetAllByIdProfesionalAndIdEmpresa(fechaInicio, dia, idProfesional, _permissionsBusiness.Value.User.IdEmpresa, tran);

                if (agendaHabilitada != null && agendaHabilitada.Count > 0)
                {
                    var existenTurnosAgendados = await _profesionalesTurnosRepository.ExistenTurnosAgendadosByFecha(dia, dia.AddDays(1), idProfesional, _permissionsBusiness.Value.User.IdEmpresa, tran);
                    if (existenTurnosAgendados)
                        throw new BusinessException("Existen turnos agendados para el rango seleccionado.");

                    //Borro todo lo de la sig semana... 
                    await _profesionalesTurnosRepository.RemoveAllByFecha(dia, dia.AddDays(1), idProfesional, _permissionsBusiness.Value.User.IdEmpresa, tran);

                    //Agrego nuevos turnos
                    foreach (var aH in agendaHabilitada)
                    {
                        await _profesionalesTurnosRepository.Insert(new ProfesionalTurno()
                        {
                            IdProfesional = idProfesional,
                            IdEmpresa = _permissionsBusiness.Value.User.IdEmpresa,
                            FechaInicio = aH.FechaInicio.AddDays(1),
                            FechaFin = aH.FechaFin.AddDays(1),
                            IdEspecialidad = aH.IdEspecialidad,
                            IdEstadoProfesionalTurno = (int)EstadoProfesionalTurno.Disponible
                        }, tran);

                    }
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }
        
        private async Task ValidateTurno(int idProfesional, DateTime fechaInicio)
        {
            var turnoExistente = await _profesionalesTurnosRepository.GetByIdProfesionalAndIdEmpresaAndFecha(fechaInicio, idProfesional, _permissionsBusiness.Value.User.IdEmpresa);

            if (turnoExistente != null && turnoExistente.Count > 0)
            {
                throw new BusinessException("Ya existe un turno para el horario seleccionado.");
            }
        }

        public async Task<ProfesionalTurno> GetProfesionalTurnoById(int idProfesionalTurno)
        {
            var profesionalTurno = await _profesionalesTurnosRepository.GetById<ProfesionalTurno>(idProfesionalTurno);

            return profesionalTurno;
        }

        private async Task<int> SubirFirma(IFormFile firma, IDbTransaction tran)
        {
            using (var ms = new MemoryStream())
            {
                firma.CopyTo(ms);
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
                        Nombre = firma.FileName,
                        Tipo = file.MimeType,
                        Extension = file.Extension,
                        Contenido = fileBytes
                    }, tran);

                    return (int)idArchivo;
                }
                else
                {
                    throw new BusinessException("Una o mas imágenes inválidas.");
                }
            }
        }

        private async Task NewProveedor(Profesional profesional, IDbTransaction transaction = null)
        {
            var proveedor = new Proveedor
            {
                IdProfesional = profesional.IdProfesional,
                RazonSocial = profesional.Nombre,
                NombreFantasia = profesional.Nombre,
                IdTipoIVA = profesional.IdTipoIVA,
                CUIT = profesional.CUIT,
                NroIBr = profesional.NroIBr,
                Direccion = profesional.Direccion,
                CodigoPostal = profesional.CodigoPostal,
                Piso = profesional.Piso,
                Departamento = profesional.Departamento,
                Localidad = profesional.Localidad,
                Provincia = profesional.Provincia,
                Latitud = profesional.Latitud,
                Longitud = profesional.Longitud,
                Email = profesional.Email,
                IdTipoTelefono = profesional.IdTipoTelefono,
                Telefono = profesional.Telefono,

                EsLaboratorio = false,
                IdEstadoRegistro = (int)EstadoRegistro.Nuevo
            };

            await _proveedoresRepository.Insert(proveedor, transaction);
        }

        private async Task UpdateProveedor(Profesional profesional, IDbTransaction transaction = null)
        {
            var proveedor = await _proveedoresRepository.GetProveedorByIdProfesional(profesional.IdProfesional, transaction);

            if (proveedor == null)
            {
                await NewProveedor(profesional, transaction);
            }
            else
            {
                proveedor.RazonSocial = profesional.Nombre;
                proveedor.NombreFantasia = profesional.Nombre;
                proveedor.IdTipoIVA = profesional.IdTipoIVA;
                proveedor.CUIT = profesional.CUIT;
                proveedor.NroIBr = profesional.NroIBr;
                proveedor.Direccion = profesional.Direccion;
                proveedor.CodigoPostal = profesional.CodigoPostal;
                proveedor.Piso = profesional.Piso;
                proveedor.Departamento = profesional.Departamento;
                proveedor.Localidad = profesional.Localidad;
                proveedor.Provincia = profesional.Provincia;
                proveedor.Latitud = profesional.Latitud;
                proveedor.Longitud = profesional.Longitud;
                proveedor.Email = profesional.Email;
                proveedor.IdTipoTelefono = profesional.IdTipoTelefono;
                proveedor.Telefono = profesional.Telefono;

                await _proveedoresRepository.Update(proveedor, transaction);
            }
        }
    }
}
