using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.DepositosBanco;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IDepositosBancoBusiness
    {
        Task<DepositosBancoViewModel> Get(int idDepositoBanco);
        Task<DataTablesResponse<DepositoBanco>> GetAll(DataTablesRequest request);
        Task<IList<AsientoDetalle>> GetAsientoCustom(int id);
        Task New(DepositosBancoViewModel model);
        Task Remove(int idDepositoBanco);
        Task Update(DepositosBancoViewModel model);
        Task<bool> VerificarChequeDisponible(int idBanco, int idTipoCheque, string nroCheque);
    }
}