using Dapper.DataTables.Extensions;
using LizardCode.Framework.Infrastructure.Context;
using LizardCode.Framework.Application.Interfaces.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.Framework.Application.Interfaces.Services;
using LizardCode.SalmaSalud.Infrastructure.Repositories;
using LizardCode.SalmaSalud.Infrastructure.Services;
using System.Linq;
using LizardCode.SalmaSalud.Application.Interfaces.Services;

namespace LizardCode.SalmaSalud.Infrastructure
{
    public static class Injection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IDbContext, DbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();

            services.AddDapperDataTables<IDbContext>();
            services.AddTransient<IAlicuotasRepository, AlicuotasRepository>();
            services.AddTransient<IUsuariosRepository, UsuariosRepository>();
            services.AddTransient<IUsuariosEmpresasRepository, UsuariosEmpresasRepository>();
            services.AddTransient<IClientesRepository, ClientesRepository>();
            services.AddTransient<IEmpresasRepository, EmpresasRepository>();
            services.AddTransient<IEmpresasCertificadosRepository, EmpresasCertificadosRepository>();
            services.AddTransient<IMenuRepository, MenuRepository>();
            services.AddTransient<ICentrosCostoRepository, CentrosCostoRepository>();
            services.AddTransient<IProveedoresRepository, ProveedoresRepository>();
            services.AddTransient<IProveedoresEmpresasRepository, ProveedoresEmpresasRepository>();
            services.AddTransient<IProveedoresCodigosRetencionRepository, ProveedoresCodigosRetencionRepository>();
            services.AddTransient<ICodigosRetencionGananciasItemsRepository, CodigosRetencionGananciasItemsRepository>();
            services.AddTransient<ICodigosRetencionGananciasRepository, CodigosRetencionGananciasRepository>();
            services.AddTransient<ICodigosRetencionIngresosBrutosRepository, CodigosRetencionIngresosBrutosRepository>();
            services.AddTransient<ICodigosRetencionIVARepository, CodigosRetencionIVARepository>();
            services.AddTransient<ICodigosRetencionMonotributoRepository, CodigosRetencionMonotributoRepository>();
            services.AddTransient<ICodigosRetencionRepository, CodigosRetencionRepository>();
            services.AddTransient<ICodigosRetencionSussRepository, CodigosRetencionSussRepository>();
            services.AddTransient<IRubrosArticulosRepository, RubrosArticulosRepository>();
            services.AddTransient<IArticulosRepository, ArticulosRepository>();
            services.AddTransient<IRubrosContablesRepository, RubrosContablesRepository>();
            services.AddTransient<IBancosRepository, BancosRepository>();
            services.AddTransient<ICuentasContablesRepository, CuentasContablesRepository>();
            services.AddTransient<IChequesRepository, ChequesRepository>();
            services.AddTransient<IChequesDebitosAsientoRepository, ChequesDebitosAsientoRepository>();
            services.AddTransient<IAfipAuthRepository, AfipAuthRepository>();
            services.AddTransient<ITiposAsientosRepository, TiposAsientosRepository>();
            services.AddTransient<IEjerciciosRepository, EjerciciosRepository>();
            services.AddTransient<IAsientosRepository, AsientosRepository>();
            services.AddTransient<IAsientosDetalleRepository, AsientosDetalleRepository>();
            services.AddTransient<IMonedasRepository, MonedasRepository>();
            services.AddTransient<ICargosBancoRepository, CargosBancoRepository>();
            services.AddTransient<ICargosBancoItemsRepository, CargosBancoItemsRepository>();
            services.AddTransient<ICargosBancoAsientoRepository, CargosBancoAsientoRepository>();
            services.AddTransient<ISucursalesRepository, SucursalesRepository>();
            services.AddTransient<ISucursalesNumeracionRepository, SucursalesNumeracionRepository>();
            services.AddTransient<IMonedasFechasCambioRepository, MonedasFechasCambioRepository>();
            services.AddTransient<IComprobantesVentasRepository, ComprobantesVentasRepository>();
            services.AddTransient<IComprobantesVentasAFIPRepository, ComprobantesVentasAFIPRepository>();
            services.AddTransient<IComprobantesVentasAsientoRepository, ComprobantesVentasAsientoRepository>();
            services.AddTransient<IComprobantesVentasItemRepository, ComprobantesVentasItemRepository>();
            services.AddTransient<IComprobantesVentasTotalesRepository, ComprobantesVentasTotalesRepository>();
            services.AddTransient<IComprobantesVentasAnulacionesRepository, ComprobantesVentasAnulacionesRepository>();
            services.AddTransient<IComprobantesRepository, ComprobantesRepository>();
            services.AddTransient<IComprobantesComprasRepository, ComprobantesComprasRepository>();
            services.AddTransient<IComprobantesComprasAsientoRepository, ComprobantesComprasAsientoRepository>();
            services.AddTransient<IComprobantesComprasItemRepository, ComprobantesComprasItemRepository>();
            services.AddTransient<IComprobantesComprasTotalesRepository, ComprobantesComprasTotalesRepository>();
            services.AddTransient<IComprobantesComprasAFIPRepository, ComprobantesComprasAFIPRepository>();
            services.AddTransient<IRecibosRepository, RecibosRepository>();
            services.AddTransient<IRecibosAnticiposRepository, RecibosAnticiposRepository>();
            services.AddTransient<IRecibosAsientoRepository, RecibosAsientoRepository>();
            services.AddTransient<IRecibosComprobantesRepository, RecibosComprobantesRepository>();
            services.AddTransient<IRecibosDetalleRepository, RecibosDetalleRepository>();
            services.AddTransient<IRecibosRetencionesRepository, RecibosRetencionesRepository>();
            services.AddTransient<IDocumentosRepository, DocumentosRepository>();
            services.AddTransient<ITransferenciasRepository, TransferenciasRepository>();
            services.AddTransient<IOrdenesPagoRepository, OrdenesPagoRepository>();
            services.AddTransient<IOrdenesPagoAsientoRepository, OrdenesPagoAsientoRepository>();
            services.AddTransient<IOrdenesPagoComprobantesRepository, OrdenesPagoComprobantesRepository>();
            services.AddTransient<IOrdenesPagoDetalleRepository, OrdenesPagoDetalleRepository>();
            services.AddTransient<IOrdenesPagoRetencionesRepository, OrdenesPagoRetencionesRepository>();
            services.AddTransient<IOrdenesPagoPlanillaGastosRepository, OrdenesPagoPlanillaGastosRepository>();
            services.AddTransient<IOrdenesPagoAnticiposRepository, OrdenesPagoAnticiposRepository>();
            services.AddTransient<IPlanillaGastosRepository, PlanillaGastosRepository>();
            services.AddTransient<IPlanillaGastosItemsRepository, PlanillaGastosItemsRepository>();
            services.AddTransient<IPlanillaGastosComprobantesComprasRepository, PlanillaGastosComprobantesComprasRepository>();
            services.AddTransient<IPlanillaGastosAsientoRepository, PlanillaGastosAsientoRepository>();
            services.AddTransient<ICierreMesRepository, CierreMesRepository>();
            services.AddTransient<IAGIPRepository, AGIPRepository>();
            services.AddTransient<IARBARepository, ARBARepository>();
            services.AddTransient<IRetencionesPercepcionesRepository, RetencionesPercepcionesRepository>();
            services.AddTransient<ICondicionVentasComprasRepository, CondicionVentasComprasRepository>();
            services.AddTransient<IArticulosMovimientosStockRepository, ArticulosMovimientosStockRepository>();
            services.AddTransient<IPlantillasRepository, PlantillasRepository>();
            services.AddTransient<IPlantillasDetalleRepository, PlantillasDetalleRepository>();
            services.AddTransient<IDepositosBancoAsientoRepository, DepositosBancoAsientoRepository>();
            services.AddTransient<IDepositosBancoRepository, DepositosBancoRepository>();
            services.AddTransient<IDepositosBancoDetalleRepository, DepositosBancoDetalleRepository>();
            services.AddTransient<IAuditoriaLoginRepository, AuditoriaLoginRepository>();
            services.AddTransient<ISdoCtaCteCliRepository, SdoCtaCteCliRepository>();
            services.AddTransient<ISdoCtaCteCliComprobantesVentasRepository, SdoCtaCteCliComprobantesVentasRepository>();
            services.AddTransient<IComprobantesComprasPercepcionesRepository, ComprobantesComprasPercepcionesRepository>();
            services.AddTransient<ISdoCtaCtePrvRepository, SdoCtaCtePrvRepository>();
            services.AddTransient<ISdoCtaCtePrvComprobantesComprasRepository, SdoCtaCtePrvComprobantesComprasRepository>();
            services.AddTransient<IAsientosAperturaRepository, AsientosAperturaRepository>();
            services.AddTransient<IAsientosAperturaAsientoRepository, AsientosAperturaAsientoRepository>();
            services.AddTransient<ISaldosInicioBancosRepository, SaldosInicioBancosRepository>();
            services.AddTransient<ISaldosInicioBancosAnticiposRepository, SaldosInicioBancosAnticiposRepository>();
            services.AddTransient<ISaldosInicioBancosChequesRepository, SaldosInicioBancosChequesRepository>();
            services.AddTransient<ITiposAsientosCuentasRepository, TiposAsientosCuentasRepository>();
            services.AddTransient<ITipoDocumentoRepository, TipoDocumentoRepository>();
            services.AddTransient<ICargosBancoItemsComprobantesCompraRepository, CargosBancoItemsComprobantesCompraRepository>();


            services.AddTransient<ITiposTurnoRepository, TiposTurnoRepository>();
            services.AddTransient<IEspecialidadesRepository, EspecialidadesRepository>();
            services.AddTransient<IProfesionalesEmpresasRepository, ProfesionalesEmpresasRepository>();
            services.AddTransient<IProfesionalesRepository, ProfesionalesRepository>();
            services.AddTransient<IFinanciadoresPrestacionesRepository, FinanciadoresPrestacionesRepository>();
            services.AddTransient<IFinanciadoresPlanesRepository, FinanciadoresPlanesRepository>();
            services.AddTransient<IFinanciadoresRepository, FinanciadoresRepository>();
            services.AddTransient<IPacientesRepository, PacientesRepository>();
            services.AddTransient<IFeriadosRepository, FeriadosRepository>();
            services.AddTransient<IProfesionalesTurnosRepository, ProfesionalesTurnosRepository>();
            services.AddTransient<ITurnosRepository, TurnosRepository>();
            services.AddTransient<ITurnosHistorialRepository, TurnosHistorialRepository>();

            services.AddTransient<IConsultoriosRepository, ConsultoriosRepository>();
            services.AddTransient<IPrestacionesRepository, PrestacionesRepository>();
            services.AddTransient<IVademecumRepository, VademecumRepository>();

            services.AddTransient<IArchivosRepository, ArchivosRepository>();

            services.AddTransient<IAuditoriasChatApiRepository, AuditoriasChatApiRepository>();
            services.AddTransient<IEvolucionesRepository, EvolucionesRepository>();
            services.AddTransient<IEvolucionesPrestacionesRepository, EvolucionesPrestacionesRepository>();
            services.AddTransient<IEvolucionesOtrasPrestacionesRepository, EvolucionesOtrasPrestacionesRepository>();
            services.AddTransient<IEvolucionesOdontogramasPiezasRepository, EvolucionesOdontogramasPiezasRepository>();
			services.AddTransient<IEvolucionesOdontogramasPiezasZonasRepository, EvolucionesOdontogramasPiezasZonasRepository>();
            services.AddTransient<IEvolucionesArchivosRepository, EvolucionesArchivosRepository>();
            services.AddTransient<IEvolucionesRecetasRepository, EvolucionesRecetasRepository>();
            services.AddTransient<IEvolucionesOrdenesRepository, EvolucionesOrdenesRepository>();
            services.AddTransient<IBlockchainRepository, BlockchainRepository>();

            services.AddTransient<IPresupuestosPrestacionesRepository, PresupuestosPrestacionesRepository>();
            services.AddTransient<IPresupuestosOtrasPrestacionesRepository, PresupuestosOtrasPrestacionesRepository>();
            services.AddTransient<IPresupuestosRepository, PresupuestosRepository>();
            services.AddTransient<ILaboratoriosServiciosRepository, LaboratoriosServiciosRepository>();
            services.AddTransient<IPedidosLaboratoriosRepository, PedidosLaboratoriosRepository>();
            services.AddTransient<IPedidosLaboratoriosServiciosRepository, PedidosLaboratoriosServiciosRepository>();
            services.AddTransient<IPedidosLaboratoriosHistorialRepository, PedidosLaboratoriosHistorialRepository>();
            services.AddTransient<ILiquidacionesProfesionalesRepository, LiquidacionesProfesionalesRepository>();
            services.AddTransient<ILiquidacionesProfesionalesPrestacionesRepository, LiquidacionesProfesionalesPrestacionesRepository>();
            services.AddTransient<IGuardiasRepository, GuardiasRepository>();
            services.AddTransient<IFinanciadoresPrestacionesProfesionalesRepository, FinanciadoresPrestacionesProfesionalesRepository>();
            services.AddTransient<IPrestacionesProfesionalesRepository, PrestacionesProfesionalesRepository>();
            services.AddTransient<IFinanciadoresPadronRepository, FinanciadoresPadronRepository>();

            services.AddTransient<ITurnosSolicitudRepository, TurnosSolicitudRepository>();
            services.AddTransient<ITurnosSolicitudDiasRepository, TurnosSolicitudDiasRepository>();
            services.AddTransient<ITurnosSolicitudRangosHorariosRepository, TurnosSolicitudRangosHorariosRepository>();
            services.AddTransient<INacionalidadesRepository, NacionalidadesRepository>();
            services.AddTransient<IRangosHorariosRepository, RangosHorariosRepository>();
            services.AddTransient<IDiasRepository, DiasRepository>();

            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IBlockchainService,BlockchainService>();

            return services;
        }
    }
}
