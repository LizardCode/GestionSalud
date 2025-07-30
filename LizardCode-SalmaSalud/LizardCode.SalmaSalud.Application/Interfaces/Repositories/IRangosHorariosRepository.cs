using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface IRangosHorariosRepository
    {
        Task<IList<TRangoHorario>> GetAll<TRangoHorario>(IDbTransaction transaction = null);

        Task<TRangoHorario> GetById<TRangoHorario>(int id, IDbTransaction transaction = null);
    }
}