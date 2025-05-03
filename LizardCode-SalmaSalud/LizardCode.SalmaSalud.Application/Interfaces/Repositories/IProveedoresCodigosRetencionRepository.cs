using LizardCode.SalmaSalud.Domain.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IProveedoresCodigosRetencionRepository
    {
        Task<IList<TProveedoresCodigosRetencion>> GetAll<TProveedoresCodigosRetencion>(IDbTransaction transaction = null);

        Task<TProveedoresCodigosRetencion> GetById<TProveedoresCodigosRetencion>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TProveedoresCodigosRetencion>(TProveedoresCodigosRetencion entity, IDbTransaction transaction = null);

        Task<bool> Update<TProveedoresCodigosRetencion>(TProveedoresCodigosRetencion entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdProveedor(int idProveedor, IDbTransaction transaction = null);

        Task<IList<ProveedorCodigoRetencion>> GetAllByIdProveedor(int idProveedor, IDbTransaction transaction = null);
    }
}