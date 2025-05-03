using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.CodigosRetencion;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class CodigosRetencionBusiness: BaseBusiness, ICodigosRetencionBusiness
    {
        private readonly ILogger<CodigosRetencionBusiness> _logger;
        private readonly ICodigosRetencionRepository _codigosRetencionRepository;
        private readonly ICodigosRetencionGananciasRepository _codigosRetencionGananciasRepository;
        private readonly ICodigosRetencionGananciasItemsRepository _codigosRetencionGananciasItemsRepository;
        private readonly ICodigosRetencionIngresosBrutosRepository _codigosRetencionIngresosBrutosRepository;
        private readonly ICodigosRetencionIVARepository _codigosRetencionIVARepository;
        private readonly ICodigosRetencionMonotributoRepository _codigosRetencionMonotributoRepository;
        private readonly ICodigosRetencionSussRepository _codigosRetencionSussRepository;


        public CodigosRetencionBusiness(
            ILogger<CodigosRetencionBusiness> logger,
            ICodigosRetencionRepository codigosRetencionRepository,
            ICodigosRetencionGananciasRepository codigosRetencionGananciasRepository,
            ICodigosRetencionGananciasItemsRepository codigosRetencionGananciasItemsRepository,
            ICodigosRetencionIngresosBrutosRepository codigosRetencionIngresosBrutosRepository,
            ICodigosRetencionIVARepository codigosRetencionIVARepository,
            ICodigosRetencionMonotributoRepository codigosRetencionMonotributoRepository,
            ICodigosRetencionSussRepository codigosRetencionSussRepository)
        {
            _logger = logger;
            _codigosRetencionRepository = codigosRetencionRepository;
            _codigosRetencionGananciasRepository = codigosRetencionGananciasRepository;
            _codigosRetencionGananciasItemsRepository = codigosRetencionGananciasItemsRepository;
            _codigosRetencionIngresosBrutosRepository = codigosRetencionIngresosBrutosRepository;
            _codigosRetencionIVARepository = codigosRetencionIVARepository;
            _codigosRetencionMonotributoRepository = codigosRetencionMonotributoRepository;
            _codigosRetencionSussRepository = codigosRetencionSussRepository;
        }

        public async Task<CodigosRetencionViewModel> Get(int idCodigoRetencion)
        {
            var codigosRetencion = await _codigosRetencionRepository.GetById<CodigosRetencion>(idCodigoRetencion);

            if (codigosRetencion == null)
                return null;

            var model = _mapper.Map<CodigosRetencionViewModel>(codigosRetencion);

            switch (codigosRetencion.IdTipoRetencion)
            {
                case (int)TipoRetencion.Ganancias:
                    var retenGarancias = await _codigosRetencionGananciasRepository.GetById<CodigosRetencionGanancias>(idCodigoRetencion);
                    model.AcumulaPagos = retenGarancias.AcumulaPagos;
                    model.ImporteNoSujetoGanancias = retenGarancias.ImporteNoSujeto;
                    model.ImporteMinimoRetencionGanancias = retenGarancias.ImporteMinimoRetencion;
                    model.Items = _mapper.Map<List<CodigosRetencionDetalle>>(await _codigosRetencionGananciasItemsRepository.GetAllByIdCodigoRetencion(codigosRetencion.IdCodigoRetencion));
                    break;
                case (int)TipoRetencion.GananciasMonotributo:
                    var retenMonotributoGan = await _codigosRetencionMonotributoRepository.GetById<CodigosRetencionMonotributo>(idCodigoRetencion);
                    model.ImporteNoSujetoGanMonotributo = retenMonotributoGan.ImporteNoSujeto;
                    model.PorcentajeRetencionGanMonotributo = retenMonotributoGan.PorcentajeRetencion;
                    model.CantidadMesesGanMonotributo = retenMonotributoGan.CantidadMeses;
                    break;
                case (int)TipoRetencion.IVAMonotributo:
                    var retenIVAMonotributo = await _codigosRetencionMonotributoRepository.GetById<CodigosRetencionMonotributo>(idCodigoRetencion);
                    model.ImporteNoSujetoIVAMonotributo = retenIVAMonotributo.ImporteNoSujeto;
                    model.PorcentajeRetencionIVAMonotributo = retenIVAMonotributo.PorcentajeRetencion;
                    model.CantidadMesesIVAMonotributo = retenIVAMonotributo.CantidadMeses;
                    break;
                case (int)TipoRetencion.IVA:
                    var retenIVA = await _codigosRetencionIVARepository.GetById<CodigosRetencionIVA>(idCodigoRetencion);
                    model.ImporteNoSujetoIVA = retenIVA.ImporteNoSujeto;
                    model.PorcentajeRetencionIVA = retenIVA.PorcentajeRetencion;
                    break;
                case (int)TipoRetencion.IngresosBrutos:
                    var retenIB = await _codigosRetencionIngresosBrutosRepository.GetById<CodigosRetencionIngresosBrutos>(idCodigoRetencion);
                    model.ImporteNoSujetoIngBrutos = retenIB.ImporteNoSujeto;
                    model.PorcentajeRetencionIngBrutos = retenIB.PorcentajeRetencion;
                    model.PadronRetencionAGIP = retenIB.PadronRetencionAGIP;
                    model.PadronRetencionARBA = retenIB.PadronRetencionARBA;
                    break;
                case (int)TipoRetencion.SUSS:
                    var retenSUSS = await _codigosRetencionSussRepository.GetById<CodigosRetencionSUSS>(idCodigoRetencion);
                    model.ImporteNoSujetoSUSS = retenSUSS.ImporteNoSujeto;
                    model.PorcentajeRetencionSUSS = retenSUSS.PorcentajeRetencion;
                    break;
            }

            return model;
        }

        public async Task<Custom.CodigosRetencion> GetCustom(int idCodigoRetencion)
        {
            var codigosRetencion = await _codigosRetencionRepository.GetByIdCustom(idCodigoRetencion);
            switch (codigosRetencion.IdTipoRetencion)
            {
                case (int)TipoRetencion.Ganancias:
                    var retenGarancias = await _codigosRetencionGananciasRepository.GetById<CodigosRetencionGanancias>(idCodigoRetencion);
                    codigosRetencion.ImporteNoSujeto = retenGarancias.ImporteNoSujeto;
                    codigosRetencion.ImporteMinimoRetencion = retenGarancias.ImporteMinimoRetencion;
                    codigosRetencion.AcumulaPagos = retenGarancias.AcumulaPagos;
                    codigosRetencion.Items = _mapper.Map<List<CodigosRetencionGananciasItems>>(await _codigosRetencionGananciasItemsRepository.GetAllByIdCodigoRetencion(codigosRetencion.IdCodigoRetencion));
                    break;
                case (int)TipoRetencion.GananciasMonotributo:
                case (int)TipoRetencion.IVAMonotributo:
                    var retenMonotributo = await _codigosRetencionMonotributoRepository.GetById<CodigosRetencionMonotributo>(idCodigoRetencion);
                    codigosRetencion.ImporteNoSujeto = retenMonotributo.ImporteNoSujeto;
                    codigosRetencion.PorcentajeRetencion = retenMonotributo.PorcentajeRetencion;
                    codigosRetencion.CantidadMeses = retenMonotributo.CantidadMeses;
                    break;
                case (int)TipoRetencion.IVA:
                    var retenIVA = await _codigosRetencionIVARepository.GetById<CodigosRetencionIVA>(idCodigoRetencion);
                    codigosRetencion.ImporteNoSujeto = retenIVA.ImporteNoSujeto;
                    codigosRetencion.PorcentajeRetencion = retenIVA.PorcentajeRetencion;
                    break;
                case (int)TipoRetencion.IngresosBrutos:
                    var retenIB = await _codigosRetencionIngresosBrutosRepository.GetById<CodigosRetencionIngresosBrutos>(idCodigoRetencion);
                    codigosRetencion.ImporteNoSujeto = retenIB.ImporteNoSujeto;
                    codigosRetencion.PorcentajeRetencion = retenIB.PorcentajeRetencion;
                    codigosRetencion.PadronRetencionAGIP = retenIB.PadronRetencionAGIP;
                    codigosRetencion.PadronRetencionARBA = retenIB.PadronRetencionARBA;
                    break;
                case (int)TipoRetencion.SUSS:
                    var retenSUSS = await _codigosRetencionSussRepository.GetById<CodigosRetencionSUSS>(idCodigoRetencion);
                    codigosRetencion.ImporteNoSujeto = retenSUSS.ImporteNoSujeto;
                    codigosRetencion.PorcentajeRetencion = retenSUSS.PorcentajeRetencion;
                    break;
            }

            return codigosRetencion;
        }

        public async Task<DataTablesResponse<Custom.CodigosRetencion>> GetAll(DataTablesRequest request)
        {
            var customQuery = _codigosRetencionRepository.GetAllCustomQuery();
            var builder = _dbContext.Connection.QueryBuilder();

            return await _dataTablesService.Resolve<Custom.CodigosRetencion>(request, customQuery.Sql, customQuery.Parameters, true, staticWhere: builder.Sql);
        }

        public async Task New(CodigosRetencionViewModel model)
        {
            var codigoRetencion = _mapper.Map<CodigosRetencion>(model);
            List<CodigosRetencionGananciasItems> items = null;
            CodigosRetencionGanancias ganancias = null;
            CodigosRetencionIVA iva = null;
            CodigosRetencionMonotributo monotributo = null;
            CodigosRetencionSUSS suss = null;
            CodigosRetencionIngresosBrutos ib = null;

            switch (codigoRetencion.IdTipoRetencion)
            {
                case (int)TipoRetencion.Ganancias:
                    ganancias = new CodigosRetencionGanancias
                    {
                        ImporteMinimoRetencion = model.ImporteMinimoRetencionGanancias,
                        ImporteNoSujeto = model.ImporteNoSujetoGanancias,
                        AcumulaPagos = model.AcumulaPagos
                    };
                    items = _mapper.Map<List<CodigosRetencionGananciasItems>>(model.Items);
                    break;
                case (int)TipoRetencion.GananciasMonotributo:
                    monotributo = new CodigosRetencionMonotributo
                    {
                        ImporteNoSujeto = model.ImporteNoSujetoGanMonotributo,
                        PorcentajeRetencion = model.PorcentajeRetencionGanMonotributo,
                        CantidadMeses = model.CantidadMesesGanMonotributo
                    };
                    break;
                case (int)TipoRetencion.IVAMonotributo:
                    monotributo = new CodigosRetencionMonotributo { 
                        ImporteNoSujeto = model.ImporteNoSujetoIVAMonotributo,
                        PorcentajeRetencion = model.PorcentajeRetencionIVAMonotributo,
                        CantidadMeses = model.CantidadMesesIVAMonotributo
                    };
                    break;
                case (int)TipoRetencion.IVA:
                    iva = new CodigosRetencionIVA
                    {
                        ImporteNoSujeto = model.ImporteNoSujetoIVA,
                        PorcentajeRetencion = model.PorcentajeRetencionIVA
                    };
                    break;
                case (int)TipoRetencion.IngresosBrutos:
                    ib = new CodigosRetencionIngresosBrutos
                    {
                        ImporteNoSujeto = model.ImporteNoSujetoIngBrutos,
                        PorcentajeRetencion = model.PorcentajeRetencionIngBrutos,
                        PadronRetencionAGIP = model.PadronRetencionAGIP,
                        PadronRetencionARBA = model.PadronRetencionARBA
                    };
                    break;
                case (int)TipoRetencion.SUSS:
                    suss = new CodigosRetencionSUSS
                    {
                        ImporteNoSujeto = model.ImporteNoSujetoSUSS,
                        PorcentajeRetencion = model.PorcentajeRetencionSUSS
                    };
                    break;
            }

            Validate(codigoRetencion, ganancias, items, iva, ib, monotributo, suss);

            var tran = _uow.BeginTransaction();

            try
            {
                codigoRetencion.Descripcion = codigoRetencion.Descripcion.ToUpper().Trim();
                codigoRetencion.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;
                codigoRetencion.Regimen = codigoRetencion.Regimen.ToUpper().Trim();

                var id = await _codigosRetencionRepository.Insert(codigoRetencion, tran);

                if(ganancias != null)
                {
                    ganancias.IdCodigoRetencion = (int)id;
                    await _codigosRetencionGananciasRepository.Insert(ganancias, tran);

                    foreach (var item in items)
                    {
                        item.IdCodigoRetencion = (int)id;
                        await _codigosRetencionGananciasItemsRepository.Insert(item, tran);
                    }
                }

                if(iva != null)
                {
                    iva.IdCodigoRetencion = (int)id;
                    await _codigosRetencionIVARepository.Insert(iva, tran);
                }

                if (monotributo != null)
                {
                    monotributo.IdCodigoRetencion = (int)id;
                    await _codigosRetencionMonotributoRepository.Insert(monotributo, tran);
                }

                if (ib != null)
                {
                    ib.IdCodigoRetencion = (int)id;
                    await _codigosRetencionIngresosBrutosRepository.Insert(ib, tran);
                }

                if (suss != null)
                {
                    suss.IdCodigoRetencion = (int)id;
                    await _codigosRetencionSussRepository.Insert(suss, tran);
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

        public async Task Remove(int idCodigoRetencion)
        {
            var codigoRetencion = await _codigosRetencionRepository.GetById<CodigosRetencion>(idCodigoRetencion);

            if (codigoRetencion == null)
            {
                throw new BusinessException("Codigo de Retención inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                codigoRetencion.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

                await _codigosRetencionGananciasItemsRepository.DeleteByIdCodigoRetencion(codigoRetencion.IdCodigoRetencion, tran);
                await _codigosRetencionGananciasRepository.DeleteByIdCodigoRetencion(codigoRetencion.IdCodigoRetencion, tran);
                await _codigosRetencionIVARepository.DeleteByIdCodigoRetencion(codigoRetencion.IdCodigoRetencion, tran);
                await _codigosRetencionIngresosBrutosRepository.DeleteByIdCodigoRetencion(codigoRetencion.IdCodigoRetencion, tran);
                await _codigosRetencionMonotributoRepository.DeleteByIdCodigoRetencion(codigoRetencion.IdCodigoRetencion, tran);
                await _codigosRetencionSussRepository.DeleteByIdCodigoRetencion(codigoRetencion.IdCodigoRetencion, tran);

                await _codigosRetencionRepository.Update(codigoRetencion, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                _logger.LogError(ex, null);
                throw new InternalException();
            }
        }

        public async Task Update(CodigosRetencionViewModel model)
        {
            var codigoRetencion = _mapper.Map<CodigosRetencion>(model);
            List<CodigosRetencionGananciasItems> items = null;
            CodigosRetencionGanancias ganancias = null;
            CodigosRetencionIVA iva = null;
            CodigosRetencionMonotributo monotributo = null;
            CodigosRetencionSUSS suss = null;
            CodigosRetencionIngresosBrutos ib = null;

            switch (codigoRetencion.IdTipoRetencion)
            {
                case (int)TipoRetencion.Ganancias:
                    ganancias = new CodigosRetencionGanancias
                    {
                        ImporteMinimoRetencion = model.ImporteMinimoRetencionGanancias,
                        ImporteNoSujeto = model.ImporteNoSujetoGanancias,
                        AcumulaPagos = model.AcumulaPagos
                    };
                    items = _mapper.Map<List<CodigosRetencionGananciasItems>>(model.Items);
                    break;
                case (int)TipoRetencion.GananciasMonotributo:
                    monotributo = new CodigosRetencionMonotributo
                    {
                        ImporteNoSujeto = model.ImporteNoSujetoGanMonotributo,
                        PorcentajeRetencion = model.PorcentajeRetencionGanMonotributo,
                        CantidadMeses = model.CantidadMesesGanMonotributo
                    };
                    break;
                case (int)TipoRetencion.IVAMonotributo:
                    monotributo = new CodigosRetencionMonotributo
                    {
                        ImporteNoSujeto = model.ImporteNoSujetoIVAMonotributo,
                        PorcentajeRetencion = model.PorcentajeRetencionIVAMonotributo,
                        CantidadMeses = model.CantidadMesesIVAMonotributo
                    };
                    break;
                case (int)TipoRetencion.IVA:
                    iva = new CodigosRetencionIVA
                    {
                        ImporteNoSujeto = model.ImporteNoSujetoIVA,
                        PorcentajeRetencion = model.PorcentajeRetencionIVA
                    };
                    break;
                case (int)TipoRetencion.IngresosBrutos:
                    ib = new CodigosRetencionIngresosBrutos
                    {
                        ImporteNoSujeto = model.ImporteNoSujetoIngBrutos,
                        PorcentajeRetencion = model.PorcentajeRetencionIngBrutos,
                        PadronRetencionAGIP = model.PadronRetencionAGIP,
                        PadronRetencionARBA = model.PadronRetencionARBA
                    };
                    break;
                case (int)TipoRetencion.SUSS:
                    suss = new CodigosRetencionSUSS
                    {
                        ImporteNoSujeto = model.ImporteNoSujetoSUSS,
                        PorcentajeRetencion = model.PorcentajeRetencionSUSS
                    };
                    break;
            }

            Validate(codigoRetencion, ganancias, items, iva, ib, monotributo, suss);

            var dbCodigoRetencion = await _codigosRetencionRepository.GetById<CodigosRetencion>(codigoRetencion.IdCodigoRetencion);

            if (dbCodigoRetencion == null)
            {
                throw new BusinessException("Código de Retención Inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                dbCodigoRetencion.Descripcion = codigoRetencion.Descripcion.ToUpper().Trim();
                dbCodigoRetencion.IdEstadoRegistro = (int)EstadoRegistro.Modificado;
                dbCodigoRetencion.Regimen = codigoRetencion.Regimen.ToUpper().Trim();

                await _codigosRetencionRepository.Update(dbCodigoRetencion, tran);

                await _codigosRetencionGananciasItemsRepository.DeleteByIdCodigoRetencion(codigoRetencion.IdCodigoRetencion, tran);
                await _codigosRetencionGananciasRepository.DeleteByIdCodigoRetencion(codigoRetencion.IdCodigoRetencion, tran);
                await _codigosRetencionIVARepository.DeleteByIdCodigoRetencion(codigoRetencion.IdCodigoRetencion, tran);
                await _codigosRetencionIngresosBrutosRepository.DeleteByIdCodigoRetencion(codigoRetencion.IdCodigoRetencion, tran);
                await _codigosRetencionMonotributoRepository.DeleteByIdCodigoRetencion(codigoRetencion.IdCodigoRetencion, tran);
                await _codigosRetencionSussRepository.DeleteByIdCodigoRetencion(codigoRetencion.IdCodigoRetencion, tran);

                if (ganancias != null)
                {
                    ganancias.IdCodigoRetencion = dbCodigoRetencion.IdCodigoRetencion;
                    await _codigosRetencionGananciasRepository.Insert(ganancias, tran);

                    foreach (var item in items)
                    {
                        ganancias.IdCodigoRetencion = dbCodigoRetencion.IdCodigoRetencion;
                        await _codigosRetencionGananciasItemsRepository.Insert(item, tran);
                    }
                }

                if (iva != null)
                {
                    iva.IdCodigoRetencion = dbCodigoRetencion.IdCodigoRetencion;
                    await _codigosRetencionIVARepository.Insert(iva, tran);
                }

                if (monotributo != null)
                {
                    monotributo.IdCodigoRetencion = dbCodigoRetencion.IdCodigoRetencion;
                    await _codigosRetencionMonotributoRepository.Insert(monotributo, tran);
                }

                if (ib != null)
                {
                    ib.IdCodigoRetencion = dbCodigoRetencion.IdCodigoRetencion;
                    await _codigosRetencionIngresosBrutosRepository.Insert(ib, tran);
                }

                if (suss != null)
                {
                    suss.IdCodigoRetencion = dbCodigoRetencion.IdCodigoRetencion;
                    await _codigosRetencionSussRepository.Insert(suss, tran);
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

        private void Validate(CodigosRetencion codigoRetencion, CodigosRetencionGanancias ganancias, List<CodigosRetencionGananciasItems> items, CodigosRetencionIVA iva, CodigosRetencionIngresosBrutos ib, CodigosRetencionMonotributo monotributo, CodigosRetencionSUSS suss)
        {
            if (codigoRetencion.Descripcion.IsNull())
            {
                throw new BusinessException(nameof(codigoRetencion.Descripcion));
            }

            switch (codigoRetencion.IdTipoRetencion)
            {
                case (int)TipoRetencion.Ganancias:
                    if(ganancias.ImporteMinimoRetencion < 0)
                    {
                        throw new BusinessException("Ingrese el Importe Minimo de Retención de Ganancias");
                    }

                    if (ganancias.ImporteNoSujeto < 0)
                    {
                        throw new BusinessException("Ingrese el Importe No Sujeto de Retención de Ganancias");
                    }
                    break;
                case (int)TipoRetencion.GananciasMonotributo:
                case (int)TipoRetencion.IVAMonotributo:
                    if (monotributo.PorcentajeRetencion < 0 || monotributo.PorcentajeRetencion > 100)
                    {
                        throw new BusinessException("Ingrese un Porcentaje valido de Retención de Monotributo");
                    }

                    if (monotributo.ImporteNoSujeto < 0)
                    {
                        throw new BusinessException("Ingrese el Importe No Sujeto de Retención de Monotributo");
                    }

                    if (monotributo.CantidadMeses < 0)
                    {
                        throw new BusinessException("Ingrese la Cantidad de Meses para la Retención de Monotributo");
                    }
                    break;
                case (int)TipoRetencion.IVA:
                    if (iva.PorcentajeRetencion < 0 || iva.PorcentajeRetencion > 100)
                    {
                        throw new BusinessException("Ingrese un Porcentaje valido de Retención de I.V.A.");
                    }

                    if (iva.ImporteNoSujeto < 0)
                    {
                        throw new BusinessException("Ingrese el Importe No Sujeto de Retención de I.V.A.");
                    }
                    break;
                case (int)TipoRetencion.IngresosBrutos:
                    if (ib.PorcentajeRetencion < 0 || ib.PorcentajeRetencion > 100)
                    {
                        throw new BusinessException("Ingrese el Importe No Sujeto de Retención de Ingresos Brutos");
                    }

                    if (ib.ImporteNoSujeto < 0)
                    {
                        throw new BusinessException("Ingrese el Importe No Sujeto de Retención de Ingresos Brutos");
                    }
                    break;
                case (int)TipoRetencion.SUSS:
                    if (suss.PorcentajeRetencion < 0 || suss.PorcentajeRetencion > 100)
                    {
                        throw new BusinessException("Ingrese un Porcentaje valido de Retención de Ingresos Brutos");
                    }

                    if (suss.ImporteNoSujeto < 0)
                    {
                        throw new BusinessException("Ingrese el Importe No Sujeto de Retención de S.U.S.S.");
                    }
                    break;
            }
        }
    }
}
