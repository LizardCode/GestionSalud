using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IClientesEmpresasRepository
    {
        Task<IList<TClienteEmpresa>> GetAll<TClienteEmpresa>(IDbTransaction transaction = null);

        Task<TClienteEmpresa> GetById<TClienteEmpresa>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TClienteEmpresa>(TClienteEmpresa entity, IDbTransaction transaction = null);

        Task<bool> Update<TClienteEmpresa>(TClienteEmpresa entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdCliente(int idCliente, int idUsuario, IDbTransaction transaction = null);

        Task<IList<ClienteEmpresa>> GetAllByIdCliente(int idCliente, IDbTransaction transaction = null);
    }
}