using Dapper.DataTables.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IMayorCuentasBusiness
    {
        Task<List<Custom.MayorCuentas>> GetAll(DataTablesRequest request);
        Task<List<Custom.MayorCuentasDetalle>> GetMayorCuentaDetalle(int idCuentaContable, DateTime? fechaDesde, DateTime? fechaHasta);
    }
}