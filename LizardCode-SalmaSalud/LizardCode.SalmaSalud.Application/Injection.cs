using LizardCode.Framework.Helpers.ChatApi;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LizardCode.SalmaSalud.Application.Business;
using LizardCode.Framework.Application.Common.Extensions;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using System.Reflection;
using LizardCode.SalmaSalud.Application.Helpers;

namespace LizardCode.SalmaSalud.Application
{
    public static class Injection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLazyResolution();
            services.AddSingleton(GetConfiguredMappingConfig());
            services.AddScoped<IMapper, ServiceMapper>();
            services.AddScoped<IChatApiHelper, ChatApiHelper>();
            services.AddScoped<ILookupsBusiness, LookupsBusiness>();
            services.AddScoped<IImpresionesBusiness, ImpresionesBusiness>();
            services.AddScoped<IMailBusiness, MailBusiness>();
            services.AddScoped<IPermisosBusiness, PermisosBusiness>();
            services.AddScoped<IUsuariosBusiness, UsuariosBusiness>();
            services.AddScoped<IClientesBusiness, ClientesBusiness>();
            services.AddScoped<IEmpresasBusiness, EmpresasBusiness>();
            services.AddScoped<IMenuBusiness, MenuBusiness>();
            services.AddScoped<IProveedoresBusiness, ProveedoresBusiness>();
            services.AddScoped<ICentrosCostoBusiness, CentrosCostoBusiness>();
            services.AddScoped<IRubrosArticulosBusiness, RubrosArticulosBusiness>();
            services.AddScoped<IArticulosBusiness, ArticulosBusiness>();
            services.AddScoped<IAlicuotasBusiness, AlicuotasBusiness>();
            services.AddScoped<IRubrosContablesBusiness, RubrosContablesBusiness>();
            services.AddScoped<ICuentasContablesBusiness, CuentasContablesBusiness>();
            services.AddScoped<IBancosBusiness, BancosBusiness>();
            services.AddScoped<IChequesBusiness, ChequesBusiness>();
            services.AddScoped<ITiposAsientosBusiness, TiposAsientosBusiness>();
            services.AddScoped<IEjerciciosBusiness, EjerciciosBusiness>();
            services.AddScoped<IAsientosBusiness, AsientosBusiness>();
            services.AddScoped<ICodigosRetencionBusiness, CodigosRetencionBusiness>();
            services.AddScoped<IPlanCuentasBusiness, PlanCuentasBusiness>();
            services.AddScoped<IMonedasBusiness, MonedasBusiness>();
            services.AddScoped<IMonedasFechasCambioBusiness, MonedasFechasCambioBusiness>();
            services.AddScoped<ICargosBancoBusiness, CargosBancoBusiness>();
            services.AddScoped<ISucursalesBusiness, SucursalesBusiness>();
            services.AddScoped<IFacturacionAutomaticaBusiness, FacturacionAutomaticaBusiness>();
            services.AddScoped<IFacturacionManualBusiness, FacturacionManualBusiness>();
            services.AddScoped<ICargaAutomaticaBusiness, CargaAutomaticaBusiness>();
            services.AddScoped<ICargaManualBusiness, CargaManualBusiness>();
            services.AddScoped<ISubdiarioIVAVentasBusiness, SubdiarioIVAVentasBusiness>();
            services.AddScoped<ISubdiarioIVAComprasBusiness, SubdiarioIVAComprasBusiness>();
            services.AddScoped<IAnulaComprobantesVentaBusiness, AnulaComprobantesVentaBusiness>();
            services.AddScoped<IAnulaComprobantesCompraBusiness, AnulaComprobantesCompraBusiness>();
            services.AddScoped<IRecibosBusiness, RecibosBusiness>();
            services.AddScoped<IOrdenesPagoBusiness, OrdenesPagoBusiness>();
            services.AddScoped<IResumenCtaCteCliBusiness, ResumenCtaCteCliBusiness>();
            services.AddScoped<IResumenCtaCteProBusiness, ResumenCtaCteProBusiness>();
            services.AddScoped<ICierreMesBusiness, CierreMesBusiness>();
            services.AddScoped<IPlanillaGastosBusiness, PlanillaGastosBusiness>();
            services.AddScoped<IListadoRetencionesBusiness, ListadoRetencionesBusiness>();
            services.AddScoped<ICondicionVentasComprasBusiness, CondicionVentasComprasBusiness>();
            services.AddScoped<IFacturacionArticulosBusiness, FacturacionArticulosBusiness>();
            services.AddScoped<ICargaArticulosBusiness, CargaArticulosBusiness>();
            services.AddScoped<IPlantillasBusiness, PlantillasBusiness>();
            services.AddScoped<IDepositosBancoBusiness, DepositosBancoBusiness>();
            services.AddScoped<ISdoCtaCteCliBusiness, SdoCtaCteCliBusiness>();
            services.AddScoped<ISdoCtaCtePrvBusiness, SdoCtaCtePrvBusiness>();
            services.AddScoped<ISubdiarioVentasBusiness, SubdiarioVentasBusiness>();
            services.AddScoped<ISubdiarioComprasBusiness, SubdiarioComprasBusiness>();
            services.AddScoped<IMayorCuentasBusiness, MayorCuentasBusiness>();
            services.AddScoped<IBalancePatrimonialBusiness, BalancePatrimonialBusiness>();
            services.AddScoped<IEstadoResultadosBusiness, EstadoResultadosBusiness>();
            services.AddScoped<IBalanceSumSdoBusiness, BalanceSumSdoBusiness>();
            services.AddScoped<IAperturaAutoCuentasBusiness, AperturaAutoCuentasBusiness>();
            services.AddScoped<ISaldoInicioBancoBusiness, SaldoInicioBancoBusiness>();
			services.AddScoped<ISubdiarioCobrosBusiness, SubdiarioCobrosBusiness>();
			services.AddScoped<ISubdiarioPagosBusiness, SubdiarioPagosBusiness>();
            services.AddScoped<IProfesionalesBusiness, ProfesionalesBusiness>();
            services.AddScoped<IFinanciadoresBusiness, FinanciadoresBusiness>();
            services.AddScoped<IPacientesBusiness, PacientesBusiness>();
            services.AddScoped<IFeriadosBusiness, FeriadosBusiness>();
            services.AddScoped<IConsultoriosBusiness, ConsultoriosBusiness>();
            services.AddScoped<ITurnosBusiness, TurnosBusiness>();

            services.AddScoped<IPrestacionesBusiness, PrestacionesBusiness>();
            services.AddScoped<IPrestacionesFinanciadorBusiness, PrestacionesFinanciadorBusiness>();
            services.AddScoped<IFinanciadoresPadronBusiness, FinanciadoresPadronBusiness>();

            services.AddScoped<IChatApiBusiness, ChatApiBusiness>();
            services.AddScoped<IEvolucionesBusiness, EvolucionesBusiness>();
            services.AddScoped<IPresupuestosBusiness, PresupuestosBusiness>();
            services.AddScoped<IPedidosLaboratoriosBusiness, PedidosLaboratoriosBusiness>();
            services.AddScoped<ILiquidacionesProfesionalesBusiness, LiquidacionesProfesionalesBusiness>();
            services.AddScoped<IGuardiasBusiness, GuardiasBusiness>();

            services.AddScoped<ITurnosSolicitudBusiness, TurnosSolicitudBusiness>();
            services.AddScoped<IRangosHorariosBusiness, RangosHorariosBusiness>();
            services.AddScoped<IEspecialidadesBusiness, EspecialidadesBusiness>();
            services.AddScoped<IWAppApiHelper, WAppApiHelper>();

            return services;
        }

        private static TypeAdapterConfig GetConfiguredMappingConfig()
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());

            return config;
        }
    }
}
