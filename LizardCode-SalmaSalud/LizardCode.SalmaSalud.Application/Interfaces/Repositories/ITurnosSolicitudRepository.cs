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
    public interface ITurnosSolicitudRepository
    {
        Task<IList<TTurnoSolicitud>> GetAll<TTurnoSolicitud>(IDbTransaction transaction = null);

        Task<TTurnoSolicitud> GetById<TTurnoSolicitud>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TTurnoSolicitud>(TTurnoSolicitud entity, IDbTransaction transaction = null);

        Task<bool> Update<TTurnoSolicitud>(TTurnoSolicitud entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

        Task<Domain.EntitiesCustom.TurnoSolicitud> GetCustomById(int idTurnoSolicitud, IDbTransaction transaction = null);
        Task<List<TurnoSolicitud>> GetTurnosByIdPaciente(int idPaciente, IDbTransaction transaction = null);
        Task<TurnoSolicitudTotales> GetTotalesDashboard();
        DataTablesCustomQuery GetTurnosSolicitudDashboard();
    }
}
