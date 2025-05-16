using Dapper;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class TurnosSolicitudDiasRepository : BaseRepository, ITurnosSolicitudDiasRepository
    {
        public TurnosSolicitudDiasRepository(IDbContext context) : base(context)
        {
        }
    }
}
