using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ILaboratoriosServiciosRepository
    {
        Task<IList<TLaboratorioServicio>> GetAll<TLaboratorioServicio>(IDbTransaction transaction = null);

        Task<TLaboratorioServicio> GetById<TLaboratorioServicio>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TLaboratorioServicio>(TLaboratorioServicio entity, IDbTransaction transaction = null);

        Task<bool> Update<TLaboratorioServicio>(TLaboratorioServicio entity, IDbTransaction transaction = null);

        Task<bool> RemoveByIdLaboratorio(long idLaboratorio, IDbTransaction transaction = null);

        Task<IList<LaboratorioServicio>> GetAllByIdLaboratorio(long idLaboratorio, IDbTransaction transaction = null);

        Task<bool> RemoveById(long idLaboratorioServicio, IDbTransaction transaction = null);
    }
}
