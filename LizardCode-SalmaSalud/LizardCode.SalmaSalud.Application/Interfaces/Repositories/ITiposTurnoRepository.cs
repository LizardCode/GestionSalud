using Dapper.DataTables.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ITiposTurnoRepository
    {
        Task<IList<TTipoTurno>> GetAll<TTipoTurno>(IDbTransaction transaction = null);

        Task<TTipoTurno> GetById<TTipoTurno>(int id, IDbTransaction transaction = null);

        Task<long> Insert<TTipoTurno>(TTipoTurno entity, IDbTransaction transaction = null);

        Task<bool> Update<TTipoTurno>(TTipoTurno entity, IDbTransaction transaction = null);

        DataTablesCustomQuery GetAllCustomQuery();

    }
}