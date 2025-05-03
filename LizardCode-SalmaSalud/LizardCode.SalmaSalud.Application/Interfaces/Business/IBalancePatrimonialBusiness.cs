using Dapper.DataTables.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IBalancePatrimonialBusiness
    {
        Task<List<Custom.BalancePatrimonial>> GetAll(DataTablesRequest request);
    }
}