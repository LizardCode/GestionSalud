using System;using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IEspecialidadesRepository
    {
        Task<IList<TEspecialidad>> GetAll<TEspecialidad>(IDbTransaction transaction = null);

        Task<TEspecialidad> GetById<TEspecialidad>(int id, IDbTransaction transaction = null);
    }
}
