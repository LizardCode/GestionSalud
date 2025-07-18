using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class LookupsBusiness : ILookupsBusiness
    {
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly IProveedoresRepository _proveedoresRepository;
        private readonly IClientesRepository _clientesRepository;
        private readonly IEmpresasRepository _empresasRepository;
        private readonly ICodigosRetencionRepository _codigosRetencionRepository;
        private readonly IRubrosArticulosRepository _rubrosArticulosRepository;
        private readonly IArticulosRepository _articulosRepository;
        private readonly IAlicuotasRepository _alicuotasRepository;
        private readonly IEjerciciosRepository _ejerciciosRepository;
        private readonly IRubrosContablesRepository _rubrosContablesRepository;
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly ITiposAsientosRepository _tiposAsientosRepository;
        private readonly IMonedasRepository _monedasRepository;
        private readonly IBancosRepository _bancosRepository;
        private readonly ISucursalesRepository _sucursalesRepository;
        private readonly ITipoDocumentoRepository _tipoDocumentoRepository;
        private readonly IComprobantesRepository _comprobantesRepository;
        private readonly IComprobantesVentasRepository _comprobantesVentasRepository;
        private readonly IComprobantesComprasRepository _comprobantesComprasRepository;
        private readonly ICentrosCostoRepository _centrosCostoRepository;
        private readonly IMonedasFechasCambioRepository _monedasFechasCambioRepository;
        private readonly ICondicionVentasComprasRepository _condicionVentasComprasRepository;
		private readonly IRecibosRepository _recibosRepository;
		private readonly IOrdenesPagoRepository _ordenesPagoRepository;
		private readonly IEspecialidadesRepository _especialidadesRepository;
        private readonly IFinanciadoresRepository _financiadoresRepository;
        private readonly IProfesionalesRepository _profesionalesRepository;
        private readonly IPrestacionesRepository _prestacionesRepository;
        private readonly IConsultoriosRepository _consultoriosRepository;
        private readonly IPacientesRepository _pacientesRepository;
        private readonly IVademecumRepository _vademecumRepository;
        private readonly ITiposTurnoRepository _tiposTurnoRepository;
        private readonly INacionalidadesRepository _nacionalidadesRepository;

        public LookupsBusiness(
            IUsuariosRepository usuariosRepository,
            IClientesRepository clientesRepository,
            IEmpresasRepository empresasRepository,
            IProveedoresRepository proveedoresRepository,
            ICodigosRetencionRepository codigosRetencionRepository,
            IRubrosArticulosRepository rubrosArticulosRepository,
            IArticulosRepository articulosRepository,
            IRubrosContablesRepository rubrosContablesRepository,
            IEjerciciosRepository ejerciciosRepository,
            ITiposAsientosRepository tiposAsientosRepository,
            ICuentasContablesRepository cuentasContablesRepository,
            IBancosRepository bancosRepository,
            IMonedasRepository monedasRepository,
            ISucursalesRepository sucursalesRepository,
            IComprobantesRepository comprobantesRepository,
            IComprobantesVentasRepository comprobantesVentasRepository,
            IComprobantesComprasRepository comprobantesComprasRepository,
            ICentrosCostoRepository centrosCostoRepository,
            IMonedasFechasCambioRepository monedasFechasCambioRepository,
            IAlicuotasRepository alicuotasRepository,
			IRecibosRepository recibosRepository,
			IOrdenesPagoRepository ordenesPagoRepository,
            ITipoDocumentoRepository tipoDocumentoRepository,
            ICondicionVentasComprasRepository condicionVentasComprasRepository,
            IEspecialidadesRepository especialidadesRepository,
            IFinanciadoresRepository financiadoresRepository,
            IProfesionalesRepository profesionalesRepository,
            IPrestacionesRepository prestacionesRepository,
            IConsultoriosRepository consultoriosRepository,
            IPacientesRepository pacientesRepository,
            IVademecumRepository vademecumRepository,
            ITiposTurnoRepository tiposTurnoRepository,
            INacionalidadesRepository nacionalidadesRepository)
        {   
            _usuariosRepository = usuariosRepository;
            _clientesRepository = clientesRepository;
            _empresasRepository = empresasRepository;
            _proveedoresRepository = proveedoresRepository;
            _codigosRetencionRepository = codigosRetencionRepository;
            _rubrosArticulosRepository = rubrosArticulosRepository;
            _articulosRepository = articulosRepository;
            _rubrosContablesRepository = rubrosContablesRepository;
            _ejerciciosRepository = ejerciciosRepository;
            _alicuotasRepository = alicuotasRepository;
            _cuentasContablesRepository = cuentasContablesRepository;
            _tiposAsientosRepository = tiposAsientosRepository;
            _sucursalesRepository = sucursalesRepository;
            _recibosRepository = recibosRepository;
            _ordenesPagoRepository = ordenesPagoRepository;
			_bancosRepository = bancosRepository;
            _monedasRepository = monedasRepository;
            _comprobantesRepository = comprobantesRepository;
            _tipoDocumentoRepository = tipoDocumentoRepository;
            _comprobantesVentasRepository = comprobantesVentasRepository;
            _comprobantesComprasRepository = comprobantesComprasRepository;
            _monedasFechasCambioRepository = monedasFechasCambioRepository;
            _centrosCostoRepository = centrosCostoRepository;
            _condicionVentasComprasRepository = condicionVentasComprasRepository;
            _especialidadesRepository = especialidadesRepository;
            _financiadoresRepository = financiadoresRepository;
            _profesionalesRepository = profesionalesRepository;
            _prestacionesRepository = prestacionesRepository;
            _consultoriosRepository = consultoriosRepository;
            _pacientesRepository = pacientesRepository;
            _vademecumRepository = vademecumRepository;
            _tiposTurnoRepository = tiposTurnoRepository;
            _nacionalidadesRepository = nacionalidadesRepository;
        }

        public async Task<IList<Usuario>> GetAllUsuariosByIdEmpresaLookup(int idEmpresa) =>
            await _usuariosRepository.GetAllUsuariosByIdEmpresaLookup(idEmpresa);

        public async Task<IList<Cliente>> GetAllClientesLookup() =>
            await _clientesRepository.GetAll<Cliente>();

        public async Task<IList<Empresa>> GetAllEmpresasLookup() =>
            await _empresasRepository.GetAll<Empresa>();

        public async Task<IList<Empresa>> GetEmpresasByIdUsuarioLookup(int idUsuario) =>
            await _empresasRepository.GetAllByIdUser(idUsuario);

        public async Task<IList<RubroContable>> GetRubrosContablesByIdEmpresa(int idEmpresa) =>
            await _rubrosContablesRepository.GetRubrosContablesByIdEmpresa(idEmpresa);

        public async Task<IList<CuentaContable>> GetCuentasContablesByIdEmpresa(int idEmpresa) =>
            await _cuentasContablesRepository.GetCuentasContablesByIdEmpresa(idEmpresa);

        public async Task<IList<Proveedor>> GetAllProveedoresLookup() =>
            await _proveedoresRepository.GetAll<Proveedor>();

        public async Task<IList<CodigosRetencion>> GetAllCodigosRetencion(int? IdTipoRetencion) =>
            await _codigosRetencionRepository.GetAllByTipo<CodigosRetencion>(IdTipoRetencion);

        public async Task<IList<RubroArticulo>> GetAllRubrosArticulos(int idEmpresa) =>
            await _rubrosArticulosRepository.GetAllByIdEmpresa(idEmpresa);

        public async Task<IList<Articulo>> GetAllArticulos(int idEmpresa) =>
            await _articulosRepository.GetAllByIdEmpresa(idEmpresa);

        public async Task<IList<Alicuota>> GetAllAlicuotas() =>
            await _alicuotasRepository.GetAll<Alicuota>();

        public async Task<IList<Ejercicio>> GetEjerciciosByIdEmpresa(int idEmpresa) =>
            await _ejerciciosRepository.GetAllByIdEmpresa(idEmpresa);

        public async Task<IList<TipoAsiento>> GetTiposAsientoByIdEmpresa(int idEmpresa) =>
            await _tiposAsientosRepository.GetTiposAsientoByIdEmpresa(idEmpresa);

        public Task<IList<Banco>> GetBancosByIdEmpresa(int idEmpresa) =>
            _bancosRepository.GetBancosByIdEmpresa(idEmpresa);

        public async Task<IList<Proveedor>> GetAllProveedoresByIdEmpresaLookup(int idEmpresa) =>
            await _proveedoresRepository.GetAllProveedoresByIdEmpresaLookup(idEmpresa);

        public async Task<CuentaContable> GetCuentaContablesByIdEmpresaAndCodObservacion(int idEmpresa, int idCodigoObservacion) =>
            await _cuentasContablesRepository.GetCuentaContablesByIdEmpresaAndCodObservacion(idEmpresa, idCodigoObservacion);

        public async Task<IList<Moneda>> GetAllMonedas() =>   
            await _monedasRepository.GetAll<Moneda>();

        public async Task<IList<Sucursal>> GetAllSucursalesByIdEmpresa(int idEmpresa) =>
            await _sucursalesRepository.GetAllSucursalesByIdEmpresa(idEmpresa);

        public async Task<IList<Comprobante>> GetComprobantesByTipoIVA(int idTipoIVA) =>
            await _comprobantesRepository.GetComprobantesByTipoIVA(idTipoIVA);

        public async Task<IList<Comprobante>> GetComprobantesByCliente(int idCliente) =>
            await _comprobantesRepository.GetComprobantesByCliente(idCliente);

        public async Task<IList<Comprobante>> GetComprobantesByClienteSinCredito(int idCliente)
        {
            var result = await _comprobantesRepository.GetComprobantesByCliente(idCliente);
            return result.Where(c => !c.EsCredito).ToList();
        }
            

        public async Task<IList<Comprobante>> GetComprobantesByProveedor(int idProveedor) =>
            await _comprobantesRepository.GetComprobantesByProveedor(idProveedor);

        public async Task<IList<Comprobante>> GetComprobantesParaCredito() =>
            await _comprobantesRepository.GetComprobantesParaCredito();

        public async Task<IList<Comprobante>> GetAllComprobantes() =>
            await _comprobantesRepository.GetComprobantes();

        public async Task<double?> GetFechaCambio(string idMoneda, DateTime fecha, int idEmpresa)
        {
            if (idMoneda == Monedas.MonedaLocal.Description())
            {
                return 1D;
            }

            return await _monedasFechasCambioRepository.GetFechaCambioByCodigo(idMoneda, fecha, idEmpresa);
        }

        public async Task<double> GetFechaCambio(string idMoneda1, string idMoneda2, DateTime fecha, int idEmpresa)
        {
            if (idMoneda1 == Monedas.MonedaLocal.Description() && idMoneda2 == Monedas.MonedaLocal.Description())
            {
                return 1D;
            }

            if (idMoneda1 == Monedas.MonedaLocal.Description())
            {
                var cotizacion = await _monedasFechasCambioRepository.GetFechaCambioByCodigo(idMoneda2, fecha, idEmpresa);
                if (cotizacion == null)
                {
                    return 1D;
                }

                return cotizacion.Value;
            }

            if (idMoneda2 == Monedas.MonedaLocal.Description())
            {
                var cotizacion = await _monedasFechasCambioRepository.GetFechaCambioByCodigo(idMoneda1, fecha, idEmpresa);
                if (cotizacion == null)
                {
                    return 1D;
                }

                return cotizacion.Value;
            }

            return 1D;
        }

        public async Task<int> GetTipoIVAByComprobante(int idComprobante)
        {
            switch(idComprobante)
            {
                case (int)Comprobantes.FACTURA_A:
                case (int)Comprobantes.FACTURA_MIPYME_A:
                case (int)Comprobantes.NCREDITO_A:
                case (int)Comprobantes.NCREDITO_MIPYME_A:
                case (int)Comprobantes.NDEBITO_A:
                case (int)Comprobantes.NDEBITO_MIPYME_A:
                case (int)Comprobantes.TICKET_A:
                    return (int)TipoIVA.ResponsableInscripto;
                case (int)Comprobantes.FACTURA_B:
                case (int)Comprobantes.FACTURA_MIPYME_B:
                case (int)Comprobantes.NCREDITO_B:
                case (int)Comprobantes.NCREDITO_MIPYME_B:
                case (int)Comprobantes.NDEBITO_B:
                case (int)Comprobantes.NDEBITO_MIPYME_B:
                case (int)Comprobantes.TICKET_B:
                    return (int)TipoIVA.ConsumidorFinal;
                case (int)Comprobantes.FACTURA_C:
                case (int)Comprobantes.FACTURA_MIPYME_C:
                case (int)Comprobantes.NCREDITO_C:
                case (int)Comprobantes.NCREDITO_MIPYME_C:
                case (int)Comprobantes.NDEBITO_C:
                case (int)Comprobantes.NDEBITO_MIPYME_C:
                case (int)Comprobantes.TICKET_C:
                    return (int)TipoIVA.Monotributo;
                case (int)Comprobantes.FACTURA_E:
                case (int)Comprobantes.NCREDITO_E:
                case (int)Comprobantes.NDEBITO_E:
                    return (int)TipoIVA.Exterior;
                default:
                    return (int)TipoIVA.ConsumidorFinal;
            }
        }

        public async Task<IList<CentroCosto>> GetAllCentroCostos(int idEmpresa) =>
            await _centrosCostoRepository.GetAllByIdEmpresa(idEmpresa);

        public Task<Proveedor> GetProveedorByCUIT(string cuit, int idEmpresa) =>
            _proveedoresRepository.GetByCUIT(cuit, idEmpresa);

        public Task<DateTime?> GetMinFechaComprobanteVenta(int idEmpresa) =>
            _comprobantesVentasRepository.GetMinFechaComprobanteVenta(idEmpresa);
        
        public Task<DateTime?> GetMaxFechaComprobanteVenta(int idEmpresa) =>
            _comprobantesVentasRepository.GetMaxFechaComprobanteVenta(idEmpresa);

		public async Task<DateTime?> GetMinFechaRecibo(int idEmpresa) =>
	        await _recibosRepository.GetMinFechaRecibo(idEmpresa);

		public async Task<DateTime?> GetMaxFechaRecibo(int idEmpresa) =>
			await _recibosRepository.GetMaxFechaRecibo(idEmpresa);

		public Task<DateTime?> GetMinFechaComprobanteCompra(int idEmpresa) =>
            _comprobantesComprasRepository.GetMinFechaComprobanteCompra(idEmpresa);

        public Task<DateTime?> GetMaxFechaComprobanteCompra(int idEmpresa) =>
            _comprobantesComprasRepository.GetMaxFechaComprobanteCompra(idEmpresa);

		public async Task<DateTime?> GetMinFechaOrdenPago(int idEmpresa) =>
	        await _ordenesPagoRepository.GetMinFechaOrdenPago(idEmpresa);

		public async Task<DateTime?> GetMaxFechaOrdenPago(int idEmpresa) =>
			await _ordenesPagoRepository.GetMaxFechaOrdenPago(idEmpresa);

		public async Task<IList<CondicionVentaCompra>> GetAllCondicionVentaCompra(int? idTipoCondicion)
        {
            var condiciones = await _condicionVentasComprasRepository.GetAll<CondicionVentaCompra>();

            if (idTipoCondicion.HasValue) {
                return condiciones.ToList().Where(f => f.IdTipoCondicion == idTipoCondicion).ToList();
            }

            return condiciones;
        }

        public async Task<IList<TipoDocumentos>> GetTipoDocumentos() =>
            await _tipoDocumentoRepository.GetAll<TipoDocumentos>();

        public async Task<Usuario> ObtenerUsuario(int idUsuario) =>
            await _usuariosRepository.GetById<Usuario>(idUsuario);

        public async Task<IList<Especialidades>> GetAllEspecialidades() =>
            await _especialidadesRepository.GetAll<Especialidades>();

        public async Task<IList<Financiador>> GetAllFinanciadores() =>
            await _financiadoresRepository.GetAll<Financiador>();
        public async Task<IList<Profesional>> GetAllProfesionales(int idEmpresa) =>
            await _profesionalesRepository.GetAllProfesionalesByIdEmpresaLookup(idEmpresa);

        public async Task<IList<Prestacion>> GetAllPrestaciones() =>
            await _prestacionesRepository.GetAllPrestaciones();
        //await _prestacionesRepository.GetAll<Prestacion>();

        public async Task<IList<Consultorio>> GetAllConsultorios(int idEmpresa) =>
            await _consultoriosRepository.GetAllByIdEmpresa(idEmpresa);

        public async Task<IList<Paciente>> GetAllPacientes() =>
            await _pacientesRepository.GetAll<Paciente>();

        public async Task<IList<Vademecum>> GetAllVademecum()
        {
            var vademecums =  await _vademecumRepository.GetAll<Vademecum>();
            vademecums = vademecums.DistinctBy(d => d.Codigo).ToList();

            return vademecums;
        }

        public async Task<IList<Domain.Entities.TiposTurno>> GetTiposTurno() =>
            await _tiposTurnoRepository.GetAll<Domain.Entities.TiposTurno>();

        public async Task<IList<Domain.Entities.Nacionalidad>> GetNacionalidades() =>
            await _nacionalidadesRepository.GetAll<Domain.Entities.Nacionalidad>();
    }
}
