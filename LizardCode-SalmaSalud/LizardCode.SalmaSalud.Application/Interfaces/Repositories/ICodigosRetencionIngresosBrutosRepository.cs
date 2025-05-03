using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ICodigosRetencionIngresosBrutosRepository
    {
        Task<IList<TCodigosRetencionIngresosBrutos>> GetAll<TCodigosRetencionIngresosBrutos>(IDbTransaction transaction = null);

        Task<TCodigosRetencionIngresosBrutos> GetById<TCodigosRetencionIngresosBrutos>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TCodigosRetencionIngresosBrutos>(TCodigosRetencionIngresosBrutos entity, IDbTransaction transaction = null);

        Task<bool> Update<TCodigosRetencionIngresosBrutos>(TCodigosRetencionIngresosBrutos entity, IDbTransaction transaction = null);

        Task<bool> DeleteByIdCodigoRetencion(int idCodigoRetencion, IDbTransaction transaction = null);
    }
}