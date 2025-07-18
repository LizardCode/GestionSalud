using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Application.Interfaces.Context;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class NacionalidadesRepository : BaseRepository, INacionalidadesRepository
    {
        public NacionalidadesRepository(IDbContext context) : base(context)
        {

        }
    }
}