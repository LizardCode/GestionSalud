using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ITipoDocumentoRepository
    {
        Task<IList<TTipoDocumento>> GetAll<TTipoDocumento>(IDbTransaction transaction = null);

        Task<TTipoDocumento> GetById<TTipoDocumento>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TTipoDocumento>(TTipoDocumento entity, IDbTransaction transaction = null);

        Task<bool> Update<TTipoDocumento>(TTipoDocumento entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

    }
}