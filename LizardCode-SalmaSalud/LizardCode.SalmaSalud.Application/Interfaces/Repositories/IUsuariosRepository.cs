using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IUsuariosRepository
    {
        Task<IList<TUsuario>> GetAll<TUsuario>(IDbTransaction transaction = null);
        DataTablesCustomQuery GetAllCustomQuery();
        Task<List<Usuario>> GetAllUsuariosByIdEmpresaLookup(int idEmpresa);
        Task<Usuario> GetByDocumento(string documento);
        Task<TUsuario> GetById<TUsuario>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TUsuario>(TUsuario entity, IDbTransaction transaction = null);

        Task<bool> Update<TUsuario>(TUsuario entity, IDbTransaction transaction = null);
    }
}