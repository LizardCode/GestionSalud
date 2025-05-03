using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IClientesRepository
    {
        Task<IList<TCliente>> GetAll<TCliente>(IDbTransaction transaction = null);

        Task<TCliente> GetById<TCliente>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCliente>(TCliente entity, IDbTransaction transaction = null);

        Task<bool> Update<TCliente>(TCliente entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<bool> ValidarCUITExistente(string cuit, int? id, IDbTransaction transaction = null);

        Task<Cliente> GetClienteByCUIT(string cuit, IDbTransaction transaction = null);

        Task<Cliente> GetClienteByIdPaciente(int idPaciente, IDbTransaction transaction = null);
        Task<Cliente> GetClienteByIdFinanciador(int idFinanciador, IDbTransaction transaction = null);
        Task<Cliente> GetClienteByDocumento(string documento, IDbTransaction transaction = null);

        Task<bool> ValidarDocumentoExistente(string documento, int? id, IDbTransaction transaction = null);
    }
}