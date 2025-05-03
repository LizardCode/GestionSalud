using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IUsuariosEmpresasRepository
    {
        Task<IList<TUsuarioEmpresa>> GetAll<TUsuarioEmpresa>(IDbTransaction transaction = null);

        Task<TUsuarioEmpresa> GetById<TUsuarioEmpresa>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TUsuarioEmpresa>(TUsuarioEmpresa entity, IDbTransaction transaction = null);

        Task<bool> Update<TUsuarioEmpresa>(TUsuarioEmpresa entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdUsuario(int idUsuario, IDbTransaction transaction = null);

        Task<IList<UsuarioEmpresa>> GetAllByIdUsuario(int idUsuario, IDbTransaction transaction = null);
    }
}