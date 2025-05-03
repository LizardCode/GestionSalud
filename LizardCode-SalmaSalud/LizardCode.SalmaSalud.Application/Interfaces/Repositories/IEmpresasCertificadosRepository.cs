using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IEmpresasCertificadosRepository
    {
        Task<IList<TEmpresaCertificado>> GetAll<TEmpresaCertificado>(IDbTransaction transaction = null);

        Task<TEmpresaCertificado> GetById<TEmpresaCertificado>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TEmpresaCertificado>(TEmpresaCertificado entity, IDbTransaction transaction = null);

        Task<Domain.Entities.EmpresaCertificado> GetValidEmpresaCerificadoByIdEmpresa(int idEmpresa, IDbTransaction transaction = null);

    }
}