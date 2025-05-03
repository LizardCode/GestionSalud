using Dapper;
using LizardCode.SalmaSalud.Application.Interfaces.Business;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Application.Models.PlanCuentas;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Business
{
    public class PlanCuentasBusiness: BaseBusiness, IPlanCuentasBusiness
    {
        private readonly ICuentasContablesRepository _cuentasContablesRepository;
        private readonly IRubrosContablesRepository _rubrosContablesRepository;

        public PlanCuentasBusiness(
            IPermisosBusiness permisosBusiness,
            ICuentasContablesRepository cuentasContablesRepository,
            IRubrosContablesRepository rubrosContablesRepository)
        {
            _cuentasContablesRepository = cuentasContablesRepository;
            _rubrosContablesRepository = rubrosContablesRepository;
        }

        public async Task<PlanCuentasViewModel> Get()
        {
            var rubos = await _rubrosContablesRepository.GetRubrosContablesByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa);
            var cuentas = await _cuentasContablesRepository.GetCuentasContablesByIdEmpresa(_permissionsBusiness.Value.User.IdEmpresa);

            var model = new PlanCuentasViewModel
            {
                Rubros = rubos.AsList(),
                Cuentas = cuentas.AsList()
            };

            return model;
        }

        public async Task<PlanCuentasViewModel> GetCustom(int id)
        {
            var cuenta = await _cuentasContablesRepository.GetById<CuentaContable>(id);
            var rubro = await _rubrosContablesRepository.GetById<RubroContable>(cuenta.IdRubroContable);

            return new PlanCuentasViewModel()
            {
                IdCuentaContable = cuenta.IdCuentaContable,
                RubroContable = rubro.Descripcion,
                CodigoCuenta = cuenta.CodigoCuenta,
                Descripcion = cuenta.Descripcion
            };
        }
    }
}
