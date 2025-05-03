using Dapper.DataTables.Models;
using LizardCode.Framework.Helpers.Utilities;
using Microsoft.Extensions.Logging;
using  LizardCode.Framework.Application.Common.Exceptions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.Empresas;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;
using LizardCode.Framework.Application.Common.Enums;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.IO;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class EmpresasBusiness: BaseBusiness, IEmpresasBusiness
    {
        private List<string> _allowedImageTypes = new List<string> { "image/jpeg", "image/gif", "image/png" };

        private readonly ILogger<EmpresasBusiness> _logger;
        private readonly IEmpresasRepository _empresasRepository;
        private readonly IEmpresasCertificadosRepository _empresasCertificadosRepository;

        private readonly IRubrosContablesRepository _rubrosContablesRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly IArchivosRepository _archivosRepository;

        public EmpresasBusiness(
            ILogger<EmpresasBusiness> logger,
            IRubrosContablesRepository rubrosContablesRepository,
            ICuentasContablesRepository cuentasContablesRepository,
            IEmpresasRepository empresasRepository,
            IEmpresasCertificadosRepository empresasCertificadosRepository,
            IArchivosRepository archivosRepository)
        {
            _logger = logger;
            _empresasRepository = empresasRepository;
            _empresasCertificadosRepository = empresasCertificadosRepository;
            _rubrosContablesRepository = rubrosContablesRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
            _archivosRepository = archivosRepository;
        }


        public async Task New(EmpresaViewModel model)
        {
            var empresa = _mapper.Map<Empresa>(model);

            Validate(empresa);

            empresa.RazonSocial = empresa.RazonSocial.ToUpper().Trim();
            empresa.NombreFantasia = empresa.NombreFantasia.ToUpper().Trim();
            empresa.IdTipoIVA = empresa.IdTipoIVA;
            empresa.CUIT = empresa.CUIT.ToUpper().Trim();
            empresa.NroIBr = empresa.NroIBr.ToUpper().Trim();
            empresa.Email = empresa.Email.ToLower().Trim();
            empresa.Telefono = empresa.Telefono?.Trim() ?? string.Empty;
            empresa.Direccion = empresa.Direccion.ToUpper().Trim();
            empresa.CodigoPostal = empresa.CodigoPostal?.ToUpper().Trim() ?? string.Empty;
            empresa.Piso = empresa.Piso?.ToUpper().Trim() ?? string.Empty;
            empresa.Departamento = empresa.Departamento?.ToUpper().Trim() ?? string.Empty;
            empresa.Localidad = empresa.Localidad.ToUpper().Trim() ?? string.Empty;
            empresa.Provincia = empresa.Provincia.ToUpper().Trim() ?? string.Empty;
            empresa.IdEstadoRegistro = (int)EstadoRegistro.Nuevo;

            empresa.TurnosHoraInicio = model.TurnosHoraInicio.PadLeft(2, '0') + ":" + model.TurnosMinutosInicio.PadLeft(2, '0');
            empresa.TurnosHoraFin= model.TurnosHoraFin.PadLeft(2, '0') + ":" + model.TurnosMinutosFin.PadLeft(2, '0');
            empresa.TurnosIntervalo = int.Parse(model.TurnosIntervalo);

            var rsaKey = RSA.Create(2048);
            string subject = $"C=AR/CN={empresa.NombreFantasia}/O={empresa.NombreFantasia}/serialNumber=CUIT {empresa.CUIT}";

            // Create certificate request
            var certificateRequest = new CertificateRequest(
                subject,
                rsaKey,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            var CSRBase64 = Convert.ToBase64String(certificateRequest.CreateSigningRequest());
            var CSRBuilder = new StringBuilder();
            var offset = 0;
            var LineLength = 64;

            CSRBuilder.AppendLine("-----BEGIN CERTIFICATE REQUEST-----");

            while (offset < CSRBase64.Length)
            {
                int lineEnd = Math.Min(offset + LineLength, CSRBase64.Length);
                CSRBuilder.AppendLine(CSRBase64.Substring(offset, lineEnd - offset));
                offset = lineEnd;
            }

            CSRBuilder.AppendLine("-----END CERTIFICATE REQUEST-----");

            empresa.CSR = CSRBuilder.ToString();
            empresa.PrivateKey = Convert.ToBase64String(rsaKey.ExportRSAPrivateKey());

            var tran = _uow.BeginTransaction();

            //LOGO
            if (model.RecetaLogo != null)
            {
                empresa.IdArchivoRecetaLogo = await SubirLogo(model.RecetaLogo, tran);
            }


            await _empresasRepository.Insert(empresa, tran);

            tran.Commit();
        }

        public async Task<EmpresaViewModel> Get(int idEmpresa)
        {
            var empresa = await _empresasRepository.GetById<Empresa>(idEmpresa);

            if (empresa == null)
                return null;

            var model = _mapper.Map<EmpresaViewModel>(empresa);

            model.TurnosHoraInicio = empresa.TurnosHoraInicio.Split(":")[0];
            model.TurnosMinutosInicio = empresa.TurnosHoraInicio.Split(":")[1];
            model.TurnosHoraFin = empresa.TurnosHoraFin.Split(":")[0];
            model.TurnosMinutosFin = empresa.TurnosHoraFin.Split(":")[1];
            model.TurnosIntervalo = empresa.TurnosIntervalo.ToString();

            if (empresa.IdArchivoRecetaLogo.HasValue)
            {
                var archivo = await _archivosRepository.GetById<Archivo>(empresa.IdArchivoRecetaLogo.Value);
                if (archivo != null)
                {
                    model.UploadedRecetaLogo = Convert.ToBase64String(archivo.Contenido);
                    model.UploadedTipoRecetaLogo = archivo.Tipo;
                }
            }

            return model;
        }

        public async Task<DataTablesResponse<Custom.Empresa>> GetAll(DataTablesRequest request)
        {
            var customQuery = _empresasRepository.GetAllCustomQuery();
            return await _dataTablesService.Resolve<Custom.Empresa>(request, customQuery.Sql, customQuery.Parameters);
        }

        public async Task Update(EmpresaViewModel model)
        {
            var empresa = _mapper.Map<Empresa>(model);

            Validate(empresa);

            var dbEmpresa = await _empresasRepository.GetById<Empresa>(empresa.IdEmpresa);

            if (dbEmpresa == null)
            {
                throw new ArgumentException("Empresa inexistente");
            }

            dbEmpresa.FechaInicioActividades = empresa.FechaInicioActividades;
            dbEmpresa.RazonSocial = empresa.RazonSocial.ToUpper().Trim();
            dbEmpresa.NombreFantasia = empresa.NombreFantasia.ToUpper().Trim();
            dbEmpresa.CUIT = empresa.CUIT.ToUpper().Trim();
            dbEmpresa.IdTipoIVA = empresa.IdTipoIVA;
            dbEmpresa.NroIBr = empresa.NroIBr.ToUpper().Trim();
            dbEmpresa.IdTipoTelefono = empresa.IdTipoTelefono;
            dbEmpresa.Telefono = empresa.Telefono?.Trim() ?? String.Empty;
            dbEmpresa.Email = empresa.Email.ToLower().Trim();
            dbEmpresa.Direccion = empresa.Direccion.ToUpper().Trim();
            dbEmpresa.CodigoPostal = empresa.CodigoPostal?.ToUpper().Trim() ?? String.Empty;
            dbEmpresa.Piso = empresa.Piso?.ToUpper().Trim() ?? String.Empty;
            dbEmpresa.Departamento = empresa.Departamento?.ToUpper().Trim();
            dbEmpresa.Localidad = empresa.Localidad.ToUpper().Trim();
            dbEmpresa.Provincia = empresa.Provincia.ToUpper().Trim();
            dbEmpresa.AgentePercepcionARBA = empresa.AgentePercepcionARBA;
            dbEmpresa.AgentePercepcionAGIP = empresa.AgentePercepcionAGIP;
            dbEmpresa.EnableProdAFIP = empresa.EnableProdAFIP;
            dbEmpresa.IdEstadoRegistro = (int)EstadoRegistro.Modificado;

            dbEmpresa.TurnosHoraInicio = model.TurnosHoraInicio.PadLeft(2, '0') + ":" + model.TurnosMinutosInicio.PadLeft(2, '0');
            dbEmpresa.TurnosHoraFin = model.TurnosHoraFin.PadLeft(2, '0') + ":" + model.TurnosMinutosFin.PadLeft(2, '0');
            dbEmpresa.TurnosIntervalo = int.Parse(model.TurnosIntervalo);

            using var tran = _uow.BeginTransaction();

            //LOGO
            if (model.RemovedRecetaLogo && dbEmpresa.IdArchivoRecetaLogo.HasValue)
            {
                await _archivosRepository.DeleteById<Archivo>(dbEmpresa.IdArchivoRecetaLogo.Value, tran);
                dbEmpresa.IdArchivoRecetaLogo = null;
            }

            if (model.RecetaLogo != null)
            {
                dbEmpresa.IdArchivoRecetaLogo = await SubirLogo(model.RecetaLogo, tran);
            }

            await _empresasRepository.Update(dbEmpresa, tran);

            tran.Commit();
        }

        public async Task Remove(int idEmpresa)
        {
            var empresa = await _empresasRepository.GetById<Empresa>(idEmpresa);

            if (empresa == null)
            {
                throw new ArgumentException("Empresa inexistente");
            }

            empresa.IdEstadoRegistro = (int)EstadoRegistro.Eliminado;

            await _empresasRepository.Update(empresa);
        }


        private void Validate(Empresa empresa)
        {
            if (empresa.RazonSocial.IsNull())
            {
                throw new BusinessException("Ingrese la Razón Social de la Empresa");
            }

            if (empresa.NombreFantasia.IsNull())
            {
                throw new BusinessException("Ingrese el Nombre de Fantasia de la Empresa");
            }

            if (empresa.CUIT.IsNull() && empresa.IdTipoIVA != (int)TipoIVA.ConsumidorFinal)
            {
                throw new BusinessException("Ingrese el CUIT para la Empresa");
            }

            if (empresa.Email.IsNull())
            {
                throw new BusinessException("Ingrese una Dirección de EMail para la Empresa");
            }

            if (empresa.Direccion.IsNull())
            {
                throw new BusinessException("Ingrese una Dirección para la Empresa");
            }
        }

        public async Task<List<Empresa>> GetAllByIdUsuario(int idUsuario) 
            => await _empresasRepository.GetAllByIdUser(idUsuario);

        public async Task<Empresa> GetEmpresaById(int idEmpresa) 
        {
            var empresa = await _empresasRepository.GetById<Empresa>(idEmpresa);
            return empresa;
        }

        public async Task UploadCertificate(int idEmpresa, string crt)
        {
            var dbEmpresa = await _empresasRepository.GetById<Empresa>(idEmpresa);

            if (dbEmpresa == null)
            {
                throw new BusinessException("Empresa inexistente");
            }

            var tran = _uow.BeginTransaction();

            try
            {
                string[] formats = { "dd/MM/yyyy H:mm:ss", "MM/dd/yyyy H:mm:ss" };
                var cert = new X509Certificate2(Convert.FromBase64String(crt));

                DateTime expirationDate;
                if (DateTime.TryParseExact(cert.GetExpirationDateString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    expirationDate = date;
                }
                else
                {
                    expirationDate = DateTime.Now.AddMonths(24);
                }

                if (cert.NotAfter <= DateTime.Now)
                {
                    throw new BusinessException("El Certificado está Vencido. Genere un nuevo Certificado");
                }

                await _empresasCertificadosRepository.Insert(new EmpresaCertificado
                {
                    IdEmpresa = idEmpresa,
                    CRT = crt,
                    Vencimiento = expirationDate
                }, tran);

                tran.Commit();
            }
            catch (BusinessException ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                tran.Rollback();
                throw new InternalException();
            }
        }

        public async Task CopiarPlanCtas(int idEmpresaDestino, int idEmpresaOrigen)
        {
            var empresaOrigen = await _empresasRepository.GetById<Empresa>(idEmpresaOrigen);
            if (empresaOrigen == default)
                throw new BusinessException("Empresa Origen Inexistente");

            var empresaDestino = await _empresasRepository.GetById<Empresa>(idEmpresaDestino);
            if (empresaDestino == default)
                throw new BusinessException("Empresa Destino Inexistente");

            using var tran = _uow.BeginTransaction();

            try
            {
                var rubros = await _rubrosContablesRepository.GetRubrosContablesByIdEmpresa(empresaOrigen.IdEmpresa, tran);
                var mapeoIds = new Dictionary<int, int>();

                //Grabo el Rubro SIN Padre
                foreach (RubroContable rubro in rubros)
                {
                    var id = await _rubrosContablesRepository.Insert(new RubroContable
                    {
                        CodigoRubro = rubro.CodigoRubro,
                        Descripcion = rubro.Descripcion,
                        IdEmpresa = empresaDestino.IdEmpresa,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                        IdRubroPadre = default
                    }, tran);

                    mapeoIds[rubro.IdRubroContable] = (int)id;

                }

                //Actualizo el ID de Padre para los nuevos Rubros
                foreach (RubroContable rubro in rubros)
                {
                    if (rubro.IdRubroPadre.HasValue)
                    {
                        var newRubro = await _rubrosContablesRepository.GetById<RubroContable>(mapeoIds[rubro.IdRubroContable]);
                        newRubro.IdRubroPadre = mapeoIds[rubro.IdRubroPadre.Value];
                        await _rubrosContablesRepository.Update(newRubro, tran);
                    }
                }

                var cuentasContables = await _cuentasContablesRepository.GetCuentasContablesByIdEmpresa(empresaOrigen.IdEmpresa, tran);

                //Grabo el Plan de Cuentas con la nueva Configuracion de Rubros
                foreach (CuentaContable cuenta in cuentasContables)
                {
                    await _cuentasContablesRepository.Insert(new CuentaContable
                    {
                        CodigoCuenta = cuenta.CodigoCuenta,
                        Descripcion = cuenta.Descripcion,
                        EsCtaGastos = cuenta.EsCtaGastos,
                        IdCodigoObservacion = cuenta.IdCodigoObservacion,
                        IdEstadoRegistro = (int)EstadoRegistro.Nuevo,
                        IdEmpresa = empresaDestino.IdEmpresa,
                        IdRubroContable = mapeoIds[cuenta.IdRubroContable]
                    }, tran);
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
        private async Task<int> SubirLogo(IFormFile logo, IDbTransaction tran)
        {
            using (var ms = new MemoryStream())
            {
                logo.CopyTo(ms);
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
                        Nombre = logo.FileName,
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
    }
}
