using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IAuditoriasChatApiRepository
    {
        Task<Custom.AuditoriaChatApi> GetByIdCustom(int idAuditoriaChatApi, IDbTransaction transaction = null);
        Task<long> Insert<TAuditoria>(TAuditoria entity, IDbTransaction transaction = null);
        DataTablesCustomQuery GetAllCustomQuery(int limit = 0);
        Task<AuditoriaChatApiTotales> GetTotalesByEstado(int idEmpresa);
    }
}
