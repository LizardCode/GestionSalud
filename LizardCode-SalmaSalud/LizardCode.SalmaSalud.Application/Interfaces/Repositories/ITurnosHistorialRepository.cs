using Dapper.DataTables.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ITurnosHistorialRepository
    {
        Task<long> Insert<TTurnoHistorial>(TTurnoHistorial entity, IDbTransaction transaction = null);

        //Task<TTurnoHistorial> GetById<TTurnoHistorial>(int id, IDbTransaction transaction = null);
        //Task<TTurnoHistorial> GetCustomById<TTurnoHistorial>(int id, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();
        DataTablesCustomQuery GetHistorial(int idTurno);
    }
}
