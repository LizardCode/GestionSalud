using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPedidosLaboratoriosServiciosRepository
    {
        Task<IList<TPedidoLaboratorioServicio>> GetAll<TPedidoLaboratorioServicio>(IDbTransaction transaction = null);

        Task<TPedidoLaboratorioServicio> GetById<TPedidoLaboratorioServicio>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TPedidoLaboratorioServicio>(TPedidoLaboratorioServicio entity, IDbTransaction transaction = null);

        Task<bool> Update<TPedidoLaboratorioServicio>(TPedidoLaboratorioServicio entity, IDbTransaction transaction = null);

        Task<IList<PedidoLaboratorioServicio>> GetAllByIdPedidoLaboratorio(long idPedidoLaboratorio, IDbTransaction transaction = null);
    }
}
