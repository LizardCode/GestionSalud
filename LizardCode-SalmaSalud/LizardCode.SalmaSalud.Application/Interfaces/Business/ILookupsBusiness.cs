using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface ILookupsBusiness
    {
        Task<IList<Usuario>> GetAllUsuariosByIdEmpresaLookup(int idEmpresa);
        Task<IList<Cliente>> GetAllClientesLookup();

        Task<IList<Empresa>> GetAllEmpresasLookup();
        Task<IList<Empresa>> GetEmpresasByIdUsuarioLookup(int idUsuario);

        Task<IList<Proveedor>> GetAllProveedoresLookup();

        Task<IList<Proveedor>> GetAllProveedoresByIdEmpresaLookup(int idEmpresa);

        Task<IList<CodigosRetencion>> GetAllCodigosRetencion(int? IdTipoRetencion = null);

        Task<IList<RubroArticulo>> GetAllRubrosArticulos(int idEmpresa);

        Task<IList<Articulo>> GetAllArticulos(int idEmpresa);

        Task<IList<Alicuota>> GetAllAlicuotas();
        Task<IList<RubroContable>> GetRubrosContablesByIdEmpresa(int idEmpresa);

        Task<IList<CuentaContable>> GetCuentasContablesByIdEmpresa(int idEmpresa);

        Task<IList<Ejercicio>> GetEjerciciosByIdEmpresa(int idEmpresa);

        Task<IList<TipoAsiento>> GetTiposAsientoByIdEmpresa(int idEmpresa);

        Task<IList<Banco>> GetBancosByIdEmpresa(int idEmpresa);

        Task<IList<Moneda>> GetAllMonedas();

        Task<IList<Sucursal>> GetAllSucursalesByIdEmpresa(int idEmpresa);

        Task<IList<Comprobante>> GetComprobantesByTipoIVA(int idTipoIVA);

        Task<IList<Comprobante>> GetComprobantesByCliente(int idCliente);
        Task<IList<Comprobante>> GetComprobantesByClienteSinCredito(int idCliente);

        Task<IList<Comprobante>> GetComprobantesByProveedor(int idProveedor);

        Task<IList<Comprobante>> GetComprobantesParaCredito();

        Task<double?> GetFechaCambio(string idMoneda, DateTime fecha, int idEmpresa);

        Task<double> GetFechaCambio(string idMoneda1, string idMoneda2, DateTime fecha, int idEmpresa);

        Task<int> GetTipoIVAByComprobante(int idComprobante);

        Task<IList<Comprobante>> GetAllComprobantes();

        Task<IList<CentroCosto>> GetAllCentroCostos(int idEmpresa);

        Task<Proveedor> GetProveedorByCUIT(string cuit, int idEmpresa);

        Task<DateTime?> GetMinFechaComprobanteVenta(int idEmpresa);

        Task<DateTime?> GetMaxFechaComprobanteVenta(int idEmpresa);

        Task<DateTime?> GetMinFechaComprobanteCompra(int idEmpresa);

        Task<DateTime?> GetMaxFechaComprobanteCompra(int idEmpresa);

        Task<IList<CondicionVentaCompra>> GetAllCondicionVentaCompra(int? idTipoCondicion);

        Task<Usuario> ObtenerUsuario(int idUsuario);

        Task<IList<Especialidades>> GetAllEspecialidades();

        Task<IList<Financiador>> GetAllFinanciadores();

        Task<IList<Profesional>> GetAllProfesionales(int idEmpresa);

        Task<IList<Prestacion>> GetAllPrestaciones();

        Task<IList<Consultorio>> GetAllConsultorios(int idEmpresa);

		Task<DateTime?> GetMinFechaRecibo(int idEmpresa);

		Task<DateTime?> GetMaxFechaRecibo(int idEmpresa);

		Task<DateTime?> GetMinFechaOrdenPago(int idEmpresa);

		Task<DateTime?> GetMaxFechaOrdenPago(int idEmpresa);

        Task<IList<TipoDocumentos>> GetTipoDocumentos();
        Task<IList<Paciente>> GetAllPacientes();
        Task<IList<Vademecum>> GetAllVademecum();
        Task<IList<Domain.Entities.TiposTurno>> GetTiposTurno();
    }
}