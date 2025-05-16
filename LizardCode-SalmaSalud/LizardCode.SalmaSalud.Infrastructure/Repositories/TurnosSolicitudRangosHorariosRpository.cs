using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class TurnosSolicitudRangosHorariosRepository : BaseRepository, ITurnosSolicitudRangosHorariosRepository
    {
        public TurnosSolicitudRangosHorariosRepository(IDbContext context) : base(context)
        {
        }
    }
}
