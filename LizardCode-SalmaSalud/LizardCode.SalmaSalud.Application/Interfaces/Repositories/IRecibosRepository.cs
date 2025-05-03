using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IRecibosRepository
    {
        Task<IList<TRecibo>> GetAll<TRecibo>(IDbTransaction transaction = null);

        Task<TRecibo> GetById<TRecibo>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TRecibo>(TRecibo entity, IDbTransaction transaction = null);

        Task<bool> Update<TRecibo>(TRecibo entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<Custom.Recibo> GetByIdCustom(int idRecibo, IDbTransaction transaction = null);

		Task<List<SubdiarioCobros>> GetSubdiarioCobros(Dictionary<string, object> filters);

		Task<List<SubdiarioCobrosDetalle>> GetSubdiarioCobrosDetalle(Dictionary<string, object> filters);

        Task<List<Custom.SubdiarioImputaciones>> GetSubdiarioImputaciones(DateTime? fechaDesde, DateTime? fechaHasta, int idEmpresa);

		Task<DateTime?> GetMinFechaRecibo(int idEmpresa);

		Task<DateTime?> GetMaxFechaRecibo(int idEmpresa);
	}
}