using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.EntitiesCustom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPedidosLaboratoriosRepository
    {
        Task<IList<TPedidoLaboratorio>> GetAll<TPedidoLaboratorio>(IDbTransaction transaction = null);

        Task<TPedidoLaboratorio> GetById<TPedidoLaboratorio>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TPedidoLaboratorio>(TPedidoLaboratorio entity, IDbTransaction transaction = null);

        Task<bool> Update<TPedidoLaboratorio>(TPedidoLaboratorio entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
        Task<IList<PedidoLaboratorio>> GetPedidosPorPresupuesto(int idPresupuesto);
    }
}