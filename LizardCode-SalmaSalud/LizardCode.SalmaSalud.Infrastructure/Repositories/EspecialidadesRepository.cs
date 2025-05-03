using DapperQueryBuilder;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using Dapper;
using Dapper.DataTables.Interfaces;
using Dapper.DataTables.Models;
using DapperQueryBuilder;
using LizardCode.Framework.Helpers.AFIP;
using LizardCode.Framework.Helpers.Utilities;
using LizardCode.SalmaSalud.Application.Interfaces.Repositories;
using LizardCode.SalmaSalud.Domain.Entities;
using LizardCode.SalmaSalud.Domain.Enums;
using LizardCode.Framework.Application.Interfaces.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LizardCode.SalmaSalud.Infrastructure.Repositories
{
    public class EspecialidadesRepository : BaseRepository, IEspecialidadesRepository
    {
        public EspecialidadesRepository(IDbContext context) : base(context)
        {
        }
    }
}
