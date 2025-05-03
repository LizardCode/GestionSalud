using Dapper.DataTables.Models;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IPresupuestosRepository
    {
        Task<IList<TPresupuesto>> GetAll<TPresupuesto>(IDbTransaction transaction = null);

        Task<TPresupuesto> GetById<TPresupuesto>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TPresupuesto>(TPresupuesto entity, IDbTransaction transaction = null);

        Task<bool> Update<TPresupuesto>(TPresupuesto entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
        Task<IList<Domain.EntitiesCustom.Presupuesto>> GetPresupuestosAprobados(int idEmpresa, int idPaciente);
        Task<IList<Domain.EntitiesCustom.Presupuesto>> GetPresupuestosAprobadosDisponibles(int idEmpresa);
        Task<bool> PresupuestoEnPedido(long idPresupuesto, IDbTransaction transaction = null);
    }
}
