using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Application.Models.Cheques;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Custom = LizardCode.SalmaSalud.Domain.EntitiesCustom;

namespace LizardCode.SalmaSalud.Application.Interfaces.Business
{
    public interface IChequesBusiness
    {
        Task<ChequesViewModel> Get(int idCheque);
        Task<DataTablesResponse<Domain.EntitiesCustom.Cheque>> GetAll(DataTablesRequest request);
        Task New(ChequesViewModel model);
        Task Remove(int idCheque);
        Task<bool> ValidarNumeroCheque(int idBanco, int idTipoCheque, string nroDesde, string nroHasta);
        Task<List<Custom.ChequeTerceros>> GetChequesCartera(string term);
        Task<Cheque> GetPrimerChequeDisponible(int idBanco, int idTipoCheque);
        Task<bool> ValidarNumeroChequeDisponible(int idBanco, int idTipoCheque, string nroCheque);
        Task<List<ChequesADebitarViewModel>> GetChequesADebitar(int idBanco);
        Task Debitar(List<ChequesADebitarViewModel> model, int idEjercicio, DateTime fecha, int idBancoDebitar);
        Task<IList<Custom.AsientoDetalle>> GetAsientoCustom(int id);
        Task ReverseById(int idCheque);
        Task<IList<dynamic>> Detalle(int id);
    }
}