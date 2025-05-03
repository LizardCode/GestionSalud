using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IAsientosDetalleRepository
    {
        Task<IList<Domain.EntitiesCustom.AsientoDetalle>> GetAllByIdAsiento(int idAsiento, IDbTransaction transaction = null);

        Task<bool> Insert(AsientoDetalle entity, IDbTransaction transaction = null);

        Task<bool> Update(AsientoDetalle entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdAsiento(int idAsiento, IDbTransaction transaction = null);

        Task<bool> DeleteByIdAsientoAndItem(int idAsiento, int item, IDbTransaction transaction = null);

        Task<AsientoDetalle> GetByIdAsientoAndItem(int idAsiento, int item, IDbTransaction transaction = null);

        Task<double> GetMayorCuentasDetalleSdoInicio(int idCuentaContable, DateTime? fechaDesde, int idEmpresa);

        Task<List<Domain.EntitiesCustom.MayorCuentasDetalle>> GetMayorCuentasDetalle(int idCuentaContable, DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

        Task<List<Domain.EntitiesCustom.BalancePatrimonial>> GetBalanceCuentasByRubros(List<int> rubros, int idEmpresa, int idEjercicio, DateTime fechaHasta);

        Task<List<Domain.EntitiesCustom.EstadoResultado>> GetEstadoResultadosByRubros(List<int> rubros, int idEmpresa, int idEjercicio, DateTime fechaHasta);

        Task<List<Domain.EntitiesCustom.BalanceSumSdo>> GetBalanceSumSdoByIdEmpresa(int idEmpresa, int idEjercicio, DateTime fechaDesde, DateTime fechaHasta, int? idCuentaDesde, int? idCuentaHasta);
    }
}