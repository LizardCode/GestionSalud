using Dapper.DataTables.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IListadoRetencionesBusiness
    {
        Task<List<Custom.RetencionPercepcion>> GetAll(DataTablesRequest request);
        Task<string> GetSicore(int idTipoRetencion, DateTime? fechaDesde, DateTime? fechaHasta);
    }
}