using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Application.Interfaces.Repositories
{
    public interface ITurnosSolicitudRangosHorariosRepository
    {
        Task<long> Insert<TurnoSolicitudRangoHorario>(TurnoSolicitudRangoHorario entity, IDbTransaction transaction = null);
    }
}
